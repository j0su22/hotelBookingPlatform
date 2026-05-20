namespace HotelBookingPlatform.Application.Common.DTOs;

public sealed record RatePlanDto(
    Guid Id,
    Guid RoomTypeId,
    string RoomTypeName,
    string Name,
    decimal PricePerNight,
    DateOnly ValidFrom,
    DateOnly ValidTo,
    bool IsActive);
