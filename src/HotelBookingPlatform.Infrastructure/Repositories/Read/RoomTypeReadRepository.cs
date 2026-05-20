using Dapper;
using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HotelBookingPlatform.Infrastructure.Repositories.Read;

public sealed class RoomTypeReadRepository(IConfiguration configuration) : IRoomTypeReadRepository
{
    private SqlConnection CreateConnection() =>
        new(configuration.GetConnectionString("Default"));

    public async Task<IReadOnlyList<RoomTypeDto>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT rt.Id, rt.HotelId, h.Name AS HotelName, rt.Name, rt.Description, rt.MaxCapacity, rt.BasePrice, rt.IsActive
            FROM RoomTypes rt
            INNER JOIN Hotels h ON h.Id = rt.HotelId
            WHERE rt.HotelId = @HotelId AND rt.IsActive = 1
            ORDER BY rt.Name
            """;
        await using var conn = CreateConnection();
        var result = await conn.QueryAsync<RoomTypeDto>(sql, new { HotelId = hotelId });
        return result.ToList();
    }

    public async Task<RoomTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT rt.Id, rt.HotelId, h.Name AS HotelName, rt.Name, rt.Description, rt.MaxCapacity, rt.BasePrice, rt.IsActive
            FROM RoomTypes rt
            INNER JOIN Hotels h ON h.Id = rt.HotelId
            WHERE rt.Id = @Id
            """;
        await using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<RoomTypeDto>(sql, new { Id = id });
    }
}
