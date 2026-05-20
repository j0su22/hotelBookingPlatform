using FluentValidation;
using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.Inventory.Commands;

/// <summary>
/// Creates or updates room inventory for every date in [From, To).
/// If a record already exists its TotalRooms is updated while preserving
/// booked rooms (AvailableRooms = max(0, newTotal - alreadyBooked)).
/// </summary>
public sealed record UpsertInventoryCommand(
    Guid RoomTypeId,
    DateOnly From,
    DateOnly To,
    int TotalRooms) : IRequest<Result<int>>;   // returns number of days configured

public sealed class UpsertInventoryCommandValidator : AbstractValidator<UpsertInventoryCommand>
{
    public UpsertInventoryCommandValidator()
    {
        RuleFor(x => x.RoomTypeId).NotEmpty();
        RuleFor(x => x.From).NotEmpty();
        RuleFor(x => x.To)
            .GreaterThan(x => x.From)
            .WithMessage("To must be after From.")
            .Must((cmd, to) => to.DayNumber - cmd.From.DayNumber <= 366)
            .WithMessage("Date range cannot exceed 366 days.");
        RuleFor(x => x.TotalRooms).GreaterThan(0).LessThanOrEqualTo(500);
    }
}

public sealed class UpsertInventoryCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpsertInventoryCommand, Result<int>>
{
    public async Task<Result<int>> Handle(UpsertInventoryCommand request, CancellationToken cancellationToken)
    {
        // Verify room type exists
        var roomType = await uow.RoomTypes.GetByIdAsync(request.RoomTypeId, cancellationToken);
        if (roomType is null)
            return Result.Failure<int>("Room type not found.", "NOT_FOUND");

        // Load all existing records for the range (inclusive From, exclusive To)
        var existing = await uow.RoomInventories.GetByRoomTypeAndDateRangeAsync(
            request.RoomTypeId, request.From, request.To.AddDays(-1), cancellationToken);

        var existingByDate = existing.ToDictionary(e => e.Date);

        var toAdd    = new List<RoomInventory>();
        var toUpdate = new List<RoomInventory>();

        for (var date = request.From; date < request.To; date = date.AddDays(1))
        {
            if (existingByDate.TryGetValue(date, out var inv))
            {
                inv.UpdateTotalRooms(request.TotalRooms);
                toUpdate.Add(inv);
            }
            else
            {
                toAdd.Add(RoomInventory.Create(request.RoomTypeId, date, request.TotalRooms));
            }
        }

        uow.RoomInventories.AddRange(toAdd);
        uow.RoomInventories.UpdateRange(toUpdate);

        await uow.CommitAsync(cancellationToken);

        return Result.Success(toAdd.Count + toUpdate.Count);
    }
}
