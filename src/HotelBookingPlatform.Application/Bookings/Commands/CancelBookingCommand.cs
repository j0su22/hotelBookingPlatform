using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.Bookings.Commands;

public sealed record CancelBookingCommand(Guid Id) : IRequest<Result>;

public sealed class CancelBookingCommandHandler(IUnitOfWork uow) : IRequestHandler<CancelBookingCommand, Result>
{
    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await uow.Bookings.GetByIdAsync(request.Id, cancellationToken);
        if (booking is null)
            return Result.Failure("Reserva no encontrada.", "NOT_FOUND");

        var cancelResult = booking.Cancel();
        if (cancelResult.IsFailure)
            return cancelResult;

        var inventories = await uow.RoomInventories.GetByRoomTypeAndDateRangeAsync(
            booking.RoomTypeId, booking.CheckIn, booking.CheckOut.AddDays(-1), cancellationToken);

        foreach (var inv in inventories)
            inv.Increase();

        uow.Bookings.Update(booking);
        uow.RoomInventories.UpdateRange(inventories);
        await uow.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
