using HotelBookingPlatform.Domain.Entities;

namespace HotelBookingPlatform.Domain.Interfaces;

public interface IRoomInventoryRepository
{
    Task<IReadOnlyList<RoomInventory>> GetByRoomTypeAndDateRangeAsync(
        Guid roomTypeId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    void Add(RoomInventory inventory);
    void AddRange(IEnumerable<RoomInventory> inventories);
    void UpdateRange(IEnumerable<RoomInventory> inventories);
}
