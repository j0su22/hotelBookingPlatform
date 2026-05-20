using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Interfaces;
using HotelBookingPlatform.Infrastructure.Persistence;
using HotelBookingPlatform.Infrastructure.Repositories.Write;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingPlatform.Infrastructure.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly BookingDbContext _context;
    private readonly IMediator _mediator;

    public IHotelRepository Hotels { get; }
    public IRoomTypeRepository RoomTypes { get; }
    public IRoomInventoryRepository RoomInventories { get; }
    public IRatePlanRepository RatePlans { get; }
    public IBookingRepository Bookings { get; }
    public IGuestRepository Guests { get; }

    public UnitOfWork(BookingDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
        Hotels = new HotelRepository(context);
        RoomTypes = new RoomTypeRepository(context);
        RoomInventories = new RoomInventoryRepository(context);
        RatePlans = new RatePlanRepository(context);
        Bookings = new BookingRepository(context);
        Guests = new GuestRepository(context);
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _context.SaveChangesAsync(cancellationToken);
            await DispatchDomainEventsAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyConflictException($"Concurrency conflict detected: {ex.Message}");
        }
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in _context.ChangeTracker.Entries())
            entry.State = EntityState.Detached;
        return Task.CompletedTask;
    }

    public void Dispose() => _context.Dispose();

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var aggregates = _context.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(a => a.DomainEvents.Count != 0)
            .ToList();

        foreach (var aggregate in aggregates)
        {
            var events = aggregate.DomainEvents.ToList();
            aggregate.ClearDomainEvents();
            foreach (var domainEvent in events)
                await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}
