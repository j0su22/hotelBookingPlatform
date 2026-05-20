using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.Bookings.Commands;

public sealed record ConfirmBookingCommand(Guid Id) : IRequest<Result>;

public sealed class ConfirmBookingCommandHandler(IUnitOfWork uow) : IRequestHandler<ConfirmBookingCommand, Result>
{
    public async Task<Result> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await uow.Bookings.GetByIdAsync(request.Id, cancellationToken);
        if (booking is null)
            return Result.Failure("Reserva no encontrada.", "NOT_FOUND");

        var result = booking.Confirm();
        if (result.IsFailure)
            return result;

        uow.Bookings.Update(booking);
        await uow.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
