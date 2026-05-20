using HotelBookingPlatform.Domain.Common;

namespace HotelBookingPlatform.Domain.Events;

public sealed record BookingCancelledEvent(
    Guid BookingId,
    DateTime CancelledAt) : IDomainEvent;
