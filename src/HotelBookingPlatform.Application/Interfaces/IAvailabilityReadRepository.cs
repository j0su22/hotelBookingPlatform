using HotelBookingPlatform.Application.Common.DTOs;

namespace HotelBookingPlatform.Application.Interfaces;

public interface IAvailabilityReadRepository
{
    Task<IReadOnlyList<AvailabilityDto>> GetAvailabilityAsync(
        Guid? hotelId,
        DateOnly checkIn,
        DateOnly checkOut,
        int guests,
        CancellationToken cancellationToken = default);
}
