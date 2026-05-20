using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;
using HotelBookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingPlatform.Infrastructure.Repositories.Write;

public sealed class GuestRepository(BookingDbContext context) : IGuestRepository
{
    public async Task<Guest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Guests.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

    public async Task<Guest?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await context.Guests.FirstOrDefaultAsync(g => g.Email == email.ToLowerInvariant(), cancellationToken);

    public void Add(Guest guest) => context.Guests.Add(guest);
}
