namespace HotelBookingPlatform.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IHotelRepository Hotels { get; }
    IRoomTypeRepository RoomTypes { get; }
    IRoomInventoryRepository RoomInventories { get; }
    IRatePlanRepository RatePlans { get; }
    IBookingRepository Bookings { get; }
    IGuestRepository Guests { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
