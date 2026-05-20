using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;
using HotelBookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingPlatform.Infrastructure.Repositories.Write;

public sealed class HotelRepository(BookingDbContext context) : IHotelRepository
{
    public async Task<Hotel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Hotels.FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

    public void Add(Hotel hotel) => context.Hotels.Add(hotel);
    public void Update(Hotel hotel) => context.Hotels.Update(hotel);
    public void Delete(Hotel hotel) => context.Hotels.Remove(hotel);
}
