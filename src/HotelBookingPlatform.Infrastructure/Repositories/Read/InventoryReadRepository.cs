using Dapper;
using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HotelBookingPlatform.Infrastructure.Repositories.Read;

public sealed class InventoryReadRepository(IConfiguration configuration) : IInventoryReadRepository
{
    private SqlConnection CreateConnection() =>
        new(configuration.GetConnectionString("Default"));

    public async Task<IReadOnlyList<InventoryDto>> GetByRoomTypeAndDateRangeAsync(
        Guid roomTypeId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Date, TotalRooms, AvailableRooms
            FROM RoomInventories
            WHERE RoomTypeId = @RoomTypeId
              AND Date >= @From
              AND Date <= @To
            ORDER BY Date ASC
            """;

        await using var conn = CreateConnection();
        var rows = await conn.QueryAsync<InventoryDto>(sql, new
        {
            RoomTypeId = roomTypeId,
            From = from,
            To = to
        });
        return rows.ToList();
    }
}
