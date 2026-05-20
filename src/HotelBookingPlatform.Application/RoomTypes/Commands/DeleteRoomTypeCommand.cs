using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.RoomTypes.Commands;

public sealed record DeleteRoomTypeCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteRoomTypeCommandHandler(IUnitOfWork uow) : IRequestHandler<DeleteRoomTypeCommand, Result>
{
    public async Task<Result> Handle(DeleteRoomTypeCommand request, CancellationToken cancellationToken)
    {
        var roomType = await uow.RoomTypes.GetByIdAsync(request.Id, cancellationToken);
        if (roomType is null)
            return Result.Failure("Tipo de habitación no encontrado.", "NOT_FOUND");

        roomType.Deactivate();
        uow.RoomTypes.Update(roomType);
        await uow.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
