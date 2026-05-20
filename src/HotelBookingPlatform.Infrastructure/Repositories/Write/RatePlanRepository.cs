using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;
using HotelBookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingPlatform.Infrastructure.Repositories.Write;

public sealed class RatePlanRepository(BookingDbContext context) : IRatePlanRepository
{
    public async Task<RatePlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.RatePlans.FirstOrDefaultAsync(rp => rp.Id == id, cancellationToken);

    public async Task<IReadOnlyList<RatePlan>> GetActiveByRoomTypeAsync(Guid roomTypeId, CancellationToken cancellationToken = default) =>
        await context.RatePlans
            .Where(rp => rp.RoomTypeId == roomTypeId && rp.IsActive)
            .ToListAsync(cancellationToken);

    public void Add(RatePlan ratePlan) => context.RatePlans.Add(ratePlan);
    public void Update(RatePlan ratePlan) => context.RatePlans.Update(ratePlan);
    public void Delete(RatePlan ratePlan) => context.RatePlans.Remove(ratePlan);
}
