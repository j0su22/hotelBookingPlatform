using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;
using HotelBookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingPlatform.Infrastructure.Repositories.Write;

public sealed class RoomTypeRepository(BookingDbContext context) : IRoomTypeRepository
{
    public async Task<RoomType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == id, cancellationToken);

    public async Task<IReadOnlyList<RoomType>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default) =>
        await context.RoomTypes.Where(rt => rt.HotelId == hotelId).ToListAsync(cancellationToken);

    public void Add(RoomType roomType) => context.RoomTypes.Add(roomType);
    public void Update(RoomType roomType) => context.RoomTypes.Update(roomType);
    public void Delete(RoomType roomType) => context.RoomTypes.Remove(roomType);
}
