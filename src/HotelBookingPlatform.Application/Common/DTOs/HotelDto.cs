namespace HotelBookingPlatform.Application.Common.DTOs;

public sealed record HotelDto(
    Guid Id,
    string Name,
    string Address,
    string City,
    string Country,
    int StarRating,
    bool IsActive,
    DateTime CreatedAt);
