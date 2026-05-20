namespace HotelBookingPlatform.Application.Common.DTOs;

public sealed record AvailabilityDto(
    Guid RoomTypeId,
    string RoomTypeName,
    string Description,
    int MaxCapacity,
    int AvailableRooms,
    decimal PricePerNight,
    decimal TotalPrice,
    int Nights);
