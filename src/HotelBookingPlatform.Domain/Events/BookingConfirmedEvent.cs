using HotelBookingPlatform.Domain.Common;

namespace HotelBookingPlatform.Domain.Events;

public sealed record BookingConfirmedEvent(
    Guid BookingId,
    string ConfirmationNumber,
    DateTime ConfirmedAt) : IDomainEvent;
