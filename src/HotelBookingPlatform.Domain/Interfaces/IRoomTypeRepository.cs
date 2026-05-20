using HotelBookingPlatform.Domain.Entities;

namespace HotelBookingPlatform.Domain.Interfaces;

public interface IRoomTypeRepository
{
    Task<RoomType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoomType>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
    void Add(RoomType roomType);
    void Update(RoomType roomType);
    void Delete(RoomType roomType);
}
