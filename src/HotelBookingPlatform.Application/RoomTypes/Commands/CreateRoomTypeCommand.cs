using FluentValidation;
using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.RoomTypes.Commands;

public sealed record CreateRoomTypeCommand(
    Guid HotelId,
    string Name,
    string Description,
    int MaxCapacity,
    decimal BasePrice) : IRequest<Result<Guid>>;

public sealed class CreateRoomTypeCommandValidator : AbstractValidator<CreateRoomTypeCommand>
{
    public CreateRoomTypeCommandValidator()
    {
        RuleFor(x => x.HotelId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.MaxCapacity).GreaterThan(0);
        RuleFor(x => x.BasePrice).GreaterThan(0);
    }
}

public sealed class CreateRoomTypeCommandHandler(IUnitOfWork uow) : IRequestHandler<CreateRoomTypeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRoomTypeCommand request, CancellationToken cancellationToken)
    {
        var hotel = await uow.Hotels.GetByIdAsync(request.HotelId, cancellationToken);
        if (hotel is null)
            return Result.Failure<Guid>("Hotel no encontrado.", "NOT_FOUND");

        var roomType = RoomType.Create(request.HotelId, request.Name, request.Description, request.MaxCapacity, request.BasePrice);
        uow.RoomTypes.Add(roomType);
        await uow.CommitAsync(cancellationToken);
        return Result.Success(roomType.Id);
    }
}
