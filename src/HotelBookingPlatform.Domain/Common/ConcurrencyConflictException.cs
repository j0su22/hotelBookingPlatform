namespace HotelBookingPlatform.Domain.Common;

public sealed class ConcurrencyConflictException(string message) : Exception(message);
