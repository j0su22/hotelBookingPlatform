using Dapper;
using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HotelBookingPlatform.Infrastructure.Repositories.Read;

public sealed class AvailabilityReadRepository(IConfiguration configuration) : IAvailabilityReadRepository
{
    private SqlConnection CreateConnection() =>
        new(configuration.GetConnectionString("Default"));

    public async Task<IReadOnlyList<AvailabilityDto>> GetAvailabilityAsync(
        Guid hotelId, DateOnly checkIn, DateOnly checkOut, int guests, CancellationToken cancellationToken = default)
    {
        var nights = checkOut.DayNumber - checkIn.DayNumber;

        const string sql = """
            WITH InventorySummary AS (
                SELECT
                    ri.RoomTypeId,
                    MIN(ri.AvailableRooms) AS MinAvailableRooms,
                    COUNT(ri.Id) AS ConfiguredDays
                FROM RoomInventories ri
                WHERE ri.Date >= @CheckIn AND ri.Date < @CheckOut
                GROUP BY ri.RoomTypeId
            ),
            ActiveRates AS (
                SELECT
                    rp.RoomTypeId,
                    MIN(rp.PricePerNight) AS BestRate
                FROM RatePlans rp
                WHERE rp.IsActive = 1 AND rp.ValidFrom <= @CheckIn AND rp.ValidTo >= @CheckOut
                GROUP BY rp.RoomTypeId
            )
            SELECT
                rt.Id AS RoomTypeId,
                rt.Name AS RoomTypeName,
                rt.Description,
                rt.MaxCapacity,
                inv.MinAvailableRooms AS AvailableRooms,
                COALESCE(ar.BestRate, rt.BasePrice) AS PricePerNight,
                COALESCE(ar.BestRate, rt.BasePrice) * @Nights AS TotalPrice,
                @Nights AS Nights
            FROM RoomTypes rt
            INNER JOIN Hotels h ON h.Id = rt.HotelId
            INNER JOIN InventorySummary inv ON inv.RoomTypeId = rt.Id
            LEFT JOIN ActiveRates ar ON ar.RoomTypeId = rt.Id
            WHERE h.Id = @HotelId
              AND rt.IsActive = 1
              AND h.IsActive = 1
              AND rt.MaxCapacity >= @Guests
              AND inv.MinAvailableRooms > 0
              AND inv.ConfiguredDays = @Nights
            ORDER BY TotalPrice ASC
            """;

        await using var conn = CreateConnection();
        var result = await conn.QueryAsync<AvailabilityDto>(sql, new
        {
            HotelId = hotelId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            Guests = guests,
            Nights = nights
        });
        return result.ToList();
    }
}
