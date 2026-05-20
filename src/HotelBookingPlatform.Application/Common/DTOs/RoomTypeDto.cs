namespace HotelBookingPlatform.Application.Common.DTOs;

public sealed record RoomTypeDto(
    Guid Id,
    Guid HotelId,
    string HotelName,
    string Name,
    string Description,
    int MaxCapacity,
    decimal BasePrice,
    bool IsActive);
