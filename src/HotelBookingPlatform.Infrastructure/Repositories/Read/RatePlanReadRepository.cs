using Dapper;
using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HotelBookingPlatform.Infrastructure.Repositories.Read;

public sealed class RatePlanReadRepository(IConfiguration configuration) : IRatePlanReadRepository
{
    private SqlConnection CreateConnection() =>
        new(configuration.GetConnectionString("Default"));

    public async Task<IReadOnlyList<RatePlanDto>> GetByRoomTypeIdAsync(Guid roomTypeId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT rp.Id, rp.RoomTypeId, rt.Name AS RoomTypeName, rp.Name, rp.PricePerNight, rp.ValidFrom, rp.ValidTo, rp.IsActive
            FROM RatePlans rp
            INNER JOIN RoomTypes rt ON rt.Id = rp.RoomTypeId
            WHERE rp.RoomTypeId = @RoomTypeId
            ORDER BY rp.ValidFrom
            """;
        await using var conn = CreateConnection();
        var result = await conn.QueryAsync<RatePlanDto>(sql, new { RoomTypeId = roomTypeId });
        return result.ToList();
    }

    public async Task<RatePlanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT rp.Id, rp.RoomTypeId, rt.Name AS RoomTypeName, rp.Name, rp.PricePerNight, rp.ValidFrom, rp.ValidTo, rp.IsActive
            FROM RatePlans rp
            INNER JOIN RoomTypes rt ON rt.Id = rp.RoomTypeId
            WHERE rp.Id = @Id
            """;
        await using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<RatePlanDto>(sql, new { Id = id });
    }
}
