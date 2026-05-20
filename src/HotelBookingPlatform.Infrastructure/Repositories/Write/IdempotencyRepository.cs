using Dapper;
using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;
using HotelBookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingPlatform.Infrastructure.Repositories.Write;

public sealed class IdempotencyRepository(BookingDbContext context) : IIdempotencyRepository
{
    public async Task<IdempotencyRecord?> GetByKeyAsync(string key, CancellationToken cancellationToken = default) =>
        await context.IdempotencyRecords
            .FirstOrDefaultAsync(r => r.Key == key && r.ExpiresAt > DateTime.UtcNow, cancellationToken);

    public async Task SaveAsync(IdempotencyRecord record, CancellationToken cancellationToken = default)
    {
        context.IdempotencyRecords.Add(record);
        await context.SaveChangesAsync(cancellationToken);
    }
}
