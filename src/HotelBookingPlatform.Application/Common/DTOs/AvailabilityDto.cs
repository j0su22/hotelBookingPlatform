namespace HotelBookingPlatform.Application.Common.DTOs;

public sealed record AvailabilityDto(
    Guid RoomTypeId,
    string RoomTypeName,
    string Description,
    Guid HotelId,
    string HotelName,
    string City,
    int MaxCapacity,
    int AvailableRooms,
    decimal BestRate,
    decimal TotalPrice,
    int Nights);
