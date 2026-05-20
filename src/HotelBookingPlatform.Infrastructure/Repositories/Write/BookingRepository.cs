using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;
using HotelBookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingPlatform.Infrastructure.Repositories.Write;

public sealed class BookingRepository(BookingDbContext context) : IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public void Add(Booking booking) => context.Bookings.Add(booking);
    public void Update(Booking booking) => context.Bookings.Update(booking);
}
