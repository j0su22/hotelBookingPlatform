using FluentValidation;
using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace HotelBookingPlatform.Application.Bookings.Commands;

public sealed record CreateBookingCommand(
    string IdempotencyKey,
    Guid RoomTypeId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int GuestCount,
    string GuestFirstName,
    string GuestLastName,
    string GuestEmail,
    string GuestPhone) : IRequest<Result<CreateBookingResponse>>;

public sealed record CreateBookingResponse(Guid BookingId, string ConfirmationNumber);

public sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator(IOptions<BookingOptions> options)
    {
        RuleFor(x => x.IdempotencyKey).NotEmpty();
        RuleFor(x => x.RoomTypeId).NotEmpty();
        RuleFor(x => x.CheckIn).NotEmpty();
        RuleFor(x => x.CheckOut)
            .GreaterThan(x => x.CheckIn)
            .WithMessage("CheckOut debe ser posterior a CheckIn.")
            .Must((cmd, checkOut) =>
            {
                var nights = checkOut.DayNumber - cmd.CheckIn.DayNumber;
                return nights <= options.Value.MaxNights;
            })
            .WithMessage($"La estancia máxima es de {options.Value.MaxNights} noches.");
        RuleFor(x => x.GuestCount).GreaterThan(0);
        RuleFor(x => x.GuestFirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.GuestLastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.GuestEmail).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.GuestPhone).NotEmpty().MaximumLength(20);
    }
}

public sealed class CreateBookingCommandHandler(IUnitOfWork uow) : IRequestHandler<CreateBookingCommand, Result<CreateBookingResponse>>
{
    public async Task<Result<CreateBookingResponse>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var roomType = await uow.RoomTypes.GetByIdAsync(request.RoomTypeId, cancellationToken);
        if (roomType is null)
            return Result.Failure<CreateBookingResponse>("Tipo de habitación no encontrado.", "NOT_FOUND");

        if (!roomType.IsActive)
            return Result.Failure<CreateBookingResponse>("El tipo de habitación no está disponible.", "ROOM_TYPE_INACTIVE");

        if (request.GuestCount > roomType.MaxCapacity)
            return Result.Failure<CreateBookingResponse>(
                $"El tipo de habitación soporta máximo {roomType.MaxCapacity} huéspedes.", "CAPACITY_EXCEEDED");

        var checkIn = request.CheckIn;
        var checkOut = request.CheckOut;
        var nights = checkOut.DayNumber - checkIn.DayNumber;

        var inventories = await uow.RoomInventories.GetByRoomTypeAndDateRangeAsync(
            request.RoomTypeId, checkIn, checkOut.AddDays(-1), cancellationToken);

        var expectedDates = nights;
        if (inventories.Count < expectedDates)
            return Result.Failure<CreateBookingResponse>("No hay inventario configurado para todas las fechas solicitadas.", "INVENTORY_NOT_CONFIGURED");

        foreach (var inv in inventories)
        {
            var decreaseResult = inv.Decrease();
            if (decreaseResult.IsFailure)
                return Result.Failure<CreateBookingResponse>(decreaseResult.ErrorMessage!, "INVENTORY_INSUFFICIENT");
        }

        var activePlans = await uow.RatePlans.GetActiveByRoomTypeAsync(request.RoomTypeId, cancellationToken);
        var applicablePlan = activePlans
            .Where(p => p.ValidFrom <= checkIn && p.ValidTo >= checkOut.AddDays(-1))
            .OrderBy(p => p.PricePerNight)
            .FirstOrDefault();

        var pricePerNight = applicablePlan?.PricePerNight ?? roomType.BasePrice;
        var totalPrice = pricePerNight * nights;

        var guest = await uow.Guests.GetByEmailAsync(request.GuestEmail, cancellationToken);
        if (guest is null)
        {
            guest = Guest.Create(request.GuestFirstName, request.GuestLastName, request.GuestEmail, request.GuestPhone);
            uow.Guests.Add(guest);
        }

        var booking = Booking.Create(
            guest.Id,
            request.RoomTypeId,
            checkIn,
            checkOut,
            totalPrice,
            request.IdempotencyKey,
            request.GuestCount);

        uow.Bookings.Add(booking);
        uow.RoomInventories.UpdateRange(inventories);

        try
        {
            await uow.CommitAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            return Result.Failure<CreateBookingResponse>(
                "Las habitaciones ya fueron reservadas por otro usuario. Por favor intente de nuevo.",
                "INVENTORY_CONFLICT");
        }

        return Result.Success(new CreateBookingResponse(booking.Id, booking.ConfirmationNumber));
    }
}

public sealed class BookingOptions
{
    public int MaxNights { get; set; } = 30;
}
