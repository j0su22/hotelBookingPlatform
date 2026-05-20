using FluentValidation;
using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using HotelBookingPlatform.Domain.Common;
using MediatR;

namespace HotelBookingPlatform.Application.Availability.Queries;

public sealed record GetAvailabilityQuery(
    Guid? HotelId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Guests) : IRequest<Result<IReadOnlyList<AvailabilityDto>>>;

public sealed class GetAvailabilityQueryValidator : AbstractValidator<GetAvailabilityQuery>
{
    public GetAvailabilityQueryValidator()
    {
        // HotelId is optional — omit it to search across all hotels
        RuleFor(x => x.CheckIn).NotEmpty();
        RuleFor(x => x.CheckOut).GreaterThan(x => x.CheckIn)
            .WithMessage("CheckOut must be after CheckIn.");
        RuleFor(x => x.Guests).GreaterThan(0).WithMessage("At least 1 guest is required.");
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
