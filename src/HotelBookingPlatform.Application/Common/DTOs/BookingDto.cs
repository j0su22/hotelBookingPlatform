namespace HotelBookingPlatform.Application.Common.DTOs;

public sealed record BookingDto(
    Guid Id,
    string ConfirmationNumber,
    string GuestName,
    string GuestEmail,
    string HotelName,
    string RoomTypeName,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Nights,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAt);

public sealed record BookingDetailDto(
    Guid Id,
    string ConfirmationNumber,
    Guid GuestId,
    string GuestFirstName,
    string GuestLastName,
    string GuestEmail,
    string GuestPhone,
    Guid HotelId,
    string HotelName,
    Guid RoomTypeId,
    string RoomTypeName,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Nights,
    int GuestCount,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
