using HotelBookingPlatform.Application.Common.DTOs;

namespace HotelBookingPlatform.Application.Interfaces;

public interface IRatePlanReadRepository
{
    Task<IReadOnlyList<RatePlanDto>> GetByRoomTypeIdAsync(Guid roomTypeId, CancellationToken cancellationToken = default);
    Task<RatePlanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
