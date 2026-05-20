using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.Hotels.Commands;

public sealed record DeleteHotelCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteHotelCommandHandler(IUnitOfWork uow) : IRequestHandler<DeleteHotelCommand, Result>
{
    public async Task<Result> Handle(DeleteHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = await uow.Hotels.GetByIdAsync(request.Id, cancellationToken);
        if (hotel is null)
            return Result.Failure("Hotel no encontrado.", "NOT_FOUND");

        hotel.Deactivate();
        uow.Hotels.Update(hotel);
        await uow.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
