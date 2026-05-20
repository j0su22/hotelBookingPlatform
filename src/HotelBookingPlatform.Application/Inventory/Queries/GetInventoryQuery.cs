using FluentValidation;
using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using HotelBookingPlatform.Domain.Common;
using MediatR;

namespace HotelBookingPlatform.Application.Inventory.Queries;

public sealed record GetInventoryQuery(
    Guid RoomTypeId,
    DateOnly From,
    DateOnly To) : IRequest<Result<IReadOnlyList<InventoryDto>>>;

public sealed class GetInventoryQueryValidator : AbstractValidator<GetInventoryQuery>
{
    public GetInventoryQueryValidator()
    {
        RuleFor(x => x.RoomTypeId).NotEmpty();
        RuleFor(x => x.From).NotEmpty();
        RuleFor(x => x.To)
            .GreaterThan(x => x.From)
            .WithMessage("To must be after From.")
            .Must((q, to) => to.DayNumber - q.From.DayNumber <= 366)
            .WithMessage("Date range cannot exceed 366 days.");
    }
}

public sealed class GetInventoryQueryHandler(IInventoryReadRepository repo)
    : IRequestHandler<GetInventoryQuery, Result<IReadOnlyList<InventoryDto>>>
{
    public async Task<Result<IReadOnlyList<InventoryDto>>> Handle(
        GetInventoryQuery request, CancellationToken cancellationToken)
    {
        var data = await repo.GetByRoomTypeAndDateRangeAsync(
            request.RoomTypeId, request.From, request.To, cancellationToken);
        return Result.Success(data);
    }
}
