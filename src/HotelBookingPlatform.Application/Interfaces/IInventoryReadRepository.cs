using HotelBookingPlatform.Application.Common.DTOs;

namespace HotelBookingPlatform.Application.Interfaces;

public interface IInventoryReadRepository
{
    Task<IReadOnlyList<InventoryDto>> GetByRoomTypeAndDateRangeAsync(
        Guid roomTypeId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
}
