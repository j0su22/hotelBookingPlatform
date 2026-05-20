using FluentValidation;
using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using HotelBookingPlatform.Domain.Common;
using MediatR;

namespace HotelBookingPlatform.Application.Availability.Queries;

public sealed record GetAvailabilityQuery(
    Guid HotelId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Guests) : IRequest<Result<IReadOnlyList<AvailabilityDto>>>;

public sealed class GetAvailabilityQueryValidator : AbstractValidator<GetAvailabilityQuery>
{
    public GetAvailabilityQueryValidator()
    {
        RuleFor(x => x.HotelId).NotEmpty();
        RuleFor(x => x.CheckIn).NotEmpty();
        RuleFor(x => x.CheckOut).GreaterThan(x => x.CheckIn)
            .WithMessage("CheckOut debe ser posterior a CheckIn.");
        RuleFor(x => x.Guests).GreaterThan(0).WithMessage("Debe haber al menos 1 huésped.");
    }
}

public sealed class GetAvailabilityQueryHandler(IAvailabilityReadRepository repo)
    : IRequestHandler<GetAvailabilityQuery, Result<IReadOnlyList<AvailabilityDto>>>
{
    public async Task<Result<IReadOnlyList<AvailabilityDto>>> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var result = await repo.GetAvailabilityAsync(
            request.HotelId, request.CheckIn, request.CheckOut, request.Guests, cancellationToken);

        return Result.Success(result);
    }
}
