namespace HotelBookingPlatform.Application.Common.DTOs;

public sealed record InventoryDto(
    Guid Id,
    DateOnly Date,
    int TotalRooms,
    int AvailableRooms);
