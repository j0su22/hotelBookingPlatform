using HotelBookingPlatform.Application.Common.DTOs;

namespace HotelBookingPlatform.Application.Interfaces;

public interface IRoomTypeReadRepository
{
    Task<IReadOnlyList<RoomTypeDto>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
    Task<RoomTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
