using FluentValidation;
using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.RoomTypes.Commands;

public sealed record UpdateRoomTypeCommand(
    Guid Id,
    string Name,
    string Description,
    int MaxCapacity,
    decimal BasePrice) : IRequest<Result>;

public sealed class UpdateRoomTypeCommandValidator : AbstractValidator<UpdateRoomTypeCommand>
{
    public UpdateRoomTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.MaxCapacity).GreaterThan(0);
        RuleFor(x => x.BasePrice).GreaterThan(0);
    }
}

public sealed class UpdateRoomTypeCommandHandler(IUnitOfWork uow) : IRequestHandler<UpdateRoomTypeCommand, Result>
{
    public async Task<Result> Handle(UpdateRoomTypeCommand request, CancellationToken cancellationToken)
    {
        var roomType = await uow.RoomTypes.GetByIdAsync(request.Id, cancellationToken);
        if (roomType is null)
            return Result.Failure("Tipo de habitación no encontrado.", "NOT_FOUND");

        roomType.Update(request.Name, request.Description, request.MaxCapacity, request.BasePrice);
        uow.RoomTypes.Update(roomType);
        await uow.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
