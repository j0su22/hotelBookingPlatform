using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Enums;
using HotelBookingPlatform.Domain.Events;

namespace HotelBookingPlatform.Domain.Entities;

public sealed class Booking : AggregateRoot
{
    private Booking() { }

    public Guid GuestId { get; private set; }
    public Guid RoomTypeId { get; private set; }
    public DateOnly CheckIn { get; private set; }
    public DateOnly CheckOut { get; private set; }
    public BookingStatus Status { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string ConfirmationNumber { get; private set; } = default!;
    public string IdempotencyKey { get; private set; } = default!;
    public int GuestCount { get; private set; }
    public byte[] RowVersion { get; private set; } = default!;

    public Guest Guest { get; private set; } = default!;
    public RoomType RoomType { get; private set; } = default!;

    public static Booking Create(
        Guid guestId,
        Guid roomTypeId,
        DateOnly checkIn,
        DateOnly checkOut,
        decimal totalPrice,
        string idempotencyKey,
        int guestCount)
    {
        var booking = new Booking
        {
            GuestId = guestId,
            RoomTypeId = roomTypeId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
            ConfirmationNumber = GenerateConfirmationNumber(),
            IdempotencyKey = idempotencyKey,
            GuestCount = guestCount
        };

        booking.AddDomainEvent(new BookingCreatedEvent(
            booking.Id, guestId, roomTypeId, checkIn, checkOut, totalPrice));

        return booking;
    }

    public Result Confirm()
    {
        if (Status != BookingStatus.Pending)
            return Result.Failure($"No se puede confirmar una reserva en estado {Status}.", "BOOKING_INVALID_STATE");

        Status = BookingStatus.Confirmed;
        SetUpdatedAt();
        AddDomainEvent(new BookingConfirmedEvent(Id, ConfirmationNumber, DateTime.UtcNow));
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return Result.Failure("La reserva ya está cancelada.", "BOOKING_ALREADY_CANCELLED");

        Status = BookingStatus.Cancelled;
        SetUpdatedAt();
        AddDomainEvent(new BookingCancelledEvent(Id, DateTime.UtcNow));
        return Result.Success();
    }

    private static string GenerateConfirmationNumber() =>
        $"HBP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
}
