using HotelBookingPlatform.Domain.Entities;

namespace HotelBookingPlatform.Domain.Interfaces;

public interface IGuestRepository
{
    Task<Guest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guest?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Add(Guest guest);
}
