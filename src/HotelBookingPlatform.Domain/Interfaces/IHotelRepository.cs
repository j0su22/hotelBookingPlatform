using HotelBookingPlatform.Domain.Entities;

namespace HotelBookingPlatform.Domain.Interfaces;

public interface IHotelRepository
{
    Task<Hotel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Hotel hotel);
    void Update(Hotel hotel);
    void Delete(Hotel hotel);
}
