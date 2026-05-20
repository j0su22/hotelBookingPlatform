using HotelBookingPlatform.Domain.Common;

namespace HotelBookingPlatform.Domain.Events;

public sealed record BookingCreatedEvent(
    Guid BookingId,
    Guid GuestId,
    Guid RoomTypeId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    decimal TotalPrice) : IDomainEvent;
