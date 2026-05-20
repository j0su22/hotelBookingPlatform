using FluentValidation;
using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.RatePlans.Queries;

public sealed record CalculatePriceQuery(
    Guid RatePlanId,
    DateOnly CheckIn,
    DateOnly CheckOut) : IRequest<Result<decimal>>;

public sealed class CalculatePriceQueryValidator : AbstractValidator<CalculatePriceQuery>
{
    public CalculatePriceQueryValidator()
    {
        RuleFor(x => x.RatePlanId).NotEmpty();
        RuleFor(x => x.CheckOut).GreaterThan(x => x.CheckIn)
            .WithMessage("CheckOut debe ser posterior a CheckIn.");
    }
}

public sealed class CalculatePriceQueryHandler(IUnitOfWork uow) : IRequestHandler<CalculatePriceQuery, Result<decimal>>
{
    public async Task<Result<decimal>> Handle(CalculatePriceQuery request, CancellationToken cancellationToken)
    {
        var ratePlan = await uow.RatePlans.GetByIdAsync(request.RatePlanId, cancellationToken);
        if (ratePlan is null)
            return Result.Failure<decimal>("Plan de tarifas no encontrado.", "NOT_FOUND");

        return ratePlan.CalculatePrice(request.CheckIn, request.CheckOut);
    }
}
