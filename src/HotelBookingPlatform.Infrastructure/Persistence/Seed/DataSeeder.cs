using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelBookingPlatform.Infrastructure.Persistence.Seed;

public sealed class DataSeeder(BookingDbContext context, ILogger<DataSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await context.Hotels.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Database already seeded, skipping.");
            return;
        }

        logger.LogInformation("Seeding database...");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var hotel1 = Hotel.Create("Hotel Gran Atlántida", "Av. La Revolución 123", "San Salvador", "El Salvador", 5);
        var hotel2 = Hotel.Create("Hotel Vista Azul", "Blvd. Los Héroes 456", "Santa Ana", "El Salvador", 4);

        context.Hotels.AddRange(hotel1, hotel2);
        await context.SaveChangesAsync(cancellationToken);

        var roomTypes = new List<(Hotel hotel, string name, string desc, int cap, decimal price)>
        {
            (hotel1, "Suite Presidencial", "Suite de lujo con vista panorámica a la ciudad.", 4, 450m),
            (hotel1, "Habitación Doble Deluxe", "Habitación amplia con dos camas dobles.", 4, 200m),
            (hotel1, "Habitación Individual", "Habitación cómoda para viajeros de negocios.", 2, 120m),
            (hotel2, "Suite Junior", "Suite elegante con sala de estar.", 3, 280m),
            (hotel2, "Habitación Doble Estándar", "Habitación doble con todas las comodidades.", 4, 150m),
            (hotel2, "Habitación Individual Económica", "Opción económica con todas las amenidades básicas.", 2, 90m),
        };

        var createdRoomTypes = new List<RoomType>();
        foreach (var (hotel, name, desc, cap, price) in roomTypes)
        {
            var rt = RoomType.Create(hotel.Id, name, desc, cap, price);
            context.RoomTypes.Add(rt);
            createdRoomTypes.Add(rt);
        }
        await context.SaveChangesAsync(cancellationToken);

        // Inventory for next 30 days
        var inventories = new List<RoomInventory>();
        foreach (var rt in createdRoomTypes)
        {
            for (int i = 0; i < 30; i++)
            {
                inventories.Add(RoomInventory.Create(rt.Id, today.AddDays(i), totalRooms: 10));
            }
        }
        context.RoomInventories.AddRange(inventories);

        // Rate plans (valid for 30 days from today)
        var ratePlans = new List<RatePlan>();
        foreach (var rt in createdRoomTypes)
        {
            ratePlans.Add(RatePlan.Create(rt.Id, "Tarifa Estándar", rt.BasePrice, today, today.AddDays(60)));
            ratePlans.Add(RatePlan.Create(rt.Id, "Tarifa Fin de Semana", rt.BasePrice * 1.2m, today, today.AddDays(60)));
        }
        context.RatePlans.AddRange(ratePlans);
        await context.SaveChangesAsync(cancellationToken);

        // Sample guests and bookings
        var guests = new[]
        {
            Guest.Create("Juan", "Pérez", "juan.perez@example.com", "+50378901234"),
            Guest.Create("María", "López", "maria.lopez@example.com", "+50378905678"),
            Guest.Create("Carlos", "Martínez", "carlos.martinez@example.com", "+50378901122"),
            Guest.Create("Ana", "García", "ana.garcia@example.com", "+50378903344"),
            Guest.Create("Luis", "Hernández", "luis.hernandez@example.com", "+50378905566"),
        };
        context.Guests.AddRange(guests);
        await context.SaveChangesAsync(cancellationToken);

        var sampleBookings = new[]
        {
            Booking.Create(guests[0].Id, createdRoomTypes[0].Id, today.AddDays(2), today.AddDays(5), 1350m, Guid.NewGuid().ToString(), 2),
            Booking.Create(guests[1].Id, createdRoomTypes[1].Id, today.AddDays(3), today.AddDays(6), 600m, Guid.NewGuid().ToString(), 3),
            Booking.Create(guests[2].Id, createdRoomTypes[3].Id, today.AddDays(1), today.AddDays(4), 840m, Guid.NewGuid().ToString(), 2),
            Booking.Create(guests[3].Id, createdRoomTypes[4].Id, today.AddDays(5), today.AddDays(8), 450m, Guid.NewGuid().ToString(), 2),
            Booking.Create(guests[4].Id, createdRoomTypes[2].Id, today.AddDays(7), today.AddDays(9), 240m, Guid.NewGuid().ToString(), 1),
        };

        // Confirm first two bookings
        sampleBookings[0].Confirm();
        sampleBookings[1].Confirm();
        sampleBookings[0].ClearDomainEvents();
        sampleBookings[1].ClearDomainEvents();
        foreach (var b in sampleBookings)
            b.ClearDomainEvents();

        context.Bookings.AddRange(sampleBookings);

        // Decrease inventory for sample bookings
        foreach (var booking in sampleBookings)
        {
            var nights = booking.CheckOut.DayNumber - booking.CheckIn.DayNumber;
            for (int i = 0; i < nights; i++)
            {
                var date = booking.CheckIn.AddDays(i);
                var inv = inventories.FirstOrDefault(inv => inv.RoomTypeId == booking.RoomTypeId && inv.Date == date);
                inv?.Decrease();
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Database seeded successfully.");
    }
}
