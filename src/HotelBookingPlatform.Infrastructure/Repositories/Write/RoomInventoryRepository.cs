using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;
using HotelBookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingPlatform.Infrastructure.Repositories.Write;

public sealed class RoomInventoryRepository(BookingDbContext context) : IRoomInventoryRepository
{
    public async Task<IReadOnlyList<RoomInventory>> GetByRoomTypeAndDateRangeAsync(
        Guid roomTypeId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default) =>
        await context.RoomInventories
            .Where(ri => ri.RoomTypeId == roomTypeId && ri.Date >= from && ri.Date <= to)
            .OrderBy(ri => ri.Date)
            .ToListAsync(cancellationToken);

    public void Add(RoomInventory inventory) => context.RoomInventories.Add(inventory);

    public void AddRange(IEnumerable<RoomInventory> inventories) =>
        context.RoomInventories.AddRange(inventories);

    public void UpdateRange(IEnumerable<RoomInventory> inventories) =>
        context.RoomInventories.UpdateRange(inventories);
}
