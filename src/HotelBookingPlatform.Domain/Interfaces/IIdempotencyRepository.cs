using HotelBookingPlatform.Domain.Entities;

namespace HotelBookingPlatform.Domain.Interfaces;

public interface IIdempotencyRepository
{
    Task<IdempotencyRecord?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task SaveAsync(IdempotencyRecord record, CancellationToken cancellationToken = default);
}
