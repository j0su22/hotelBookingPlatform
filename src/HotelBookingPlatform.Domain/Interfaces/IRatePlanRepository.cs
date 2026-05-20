using HotelBookingPlatform.Domain.Entities;

namespace HotelBookingPlatform.Domain.Interfaces;

public interface IRatePlanRepository
{
    Task<RatePlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RatePlan>> GetActiveByRoomTypeAsync(Guid roomTypeId, CancellationToken cancellationToken = default);
    void Add(RatePlan ratePlan);
    void Update(RatePlan ratePlan);
    void Delete(RatePlan ratePlan);
}
