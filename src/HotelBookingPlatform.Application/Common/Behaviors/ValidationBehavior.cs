using FluentValidation;
using HotelBookingPlatform.Domain.Common;
using MediatR;

namespace HotelBookingPlatform.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));

        // Create a failure result of the correct type
        var resultType = typeof(TResponse);
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = resultType.GetGenericArguments()[0];
            var failureMethod = typeof(Result<>)
                .MakeGenericType(valueType)
                .GetMethod("Failure", [typeof(string), typeof(string)])!;
            return (TResponse)failureMethod.Invoke(null, [errorMessage, "VALIDATION_ERROR"])!;
        }

        return (TResponse)(object)Result.Failure(errorMessage, "VALIDATION_ERROR");
    }
}
