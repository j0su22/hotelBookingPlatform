using Dapper;
using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using HotelBookingPlatform.Domain.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HotelBookingPlatform.Infrastructure.Repositories.Read;

public sealed class HotelReadRepository(IConfiguration configuration) : IHotelReadRepository
{
    private SqlConnection CreateConnection() =>
        new(configuration.GetConnectionString("Default"));

    public async Task<PagedResult<HotelDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var allowedSortColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "Name", "City", "Country", "StarRating", "CreatedAt" };
        var sortBy = allowedSortColumns.Contains(request.SortBy) ? request.SortBy : "CreatedAt";
        var sortDir = request.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";
        var skip = (request.PageNumber - 1) * request.PageSize;

        var sql = $"""
            SELECT
                Id, Name, Address, City, Country, StarRating, IsActive, CreatedAt,
                COUNT(*) OVER() AS TotalCount
            FROM Hotels
            WHERE IsActive = 1
            ORDER BY {sortBy} {sortDir}
            OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

        await using var conn = CreateConnection();
        var rows = await conn.QueryAsync<HotelRow>(sql, new { Skip = skip, PageSize = request.PageSize });
        var list = rows.ToList();

        var totalRecords = list.Count > 0 ? list[0].TotalCount : 0;
        var data = list.Select(r => new HotelDto(r.Id, r.Name, r.Address, r.City, r.Country, r.StarRating, r.IsActive, r.CreatedAt)).ToList();

        return new PagedResult<HotelDto>
        {
            Data = data,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords
        };
    }

    public async Task<HotelDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, Name, Address, City, Country, StarRating, IsActive, CreatedAt FROM Hotels WHERE Id = @Id AND IsActive = 1";
        await using var conn = CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync<HotelRow>(sql, new { Id = id });
        if (row is null) return null;
        return new HotelDto(row.Id, row.Name, row.Address, row.City, row.Country, row.StarRating, row.IsActive, row.CreatedAt);
    }

    private sealed record HotelRow(Guid Id, string Name, string Address, string City, string Country, int StarRating, bool IsActive, DateTime CreatedAt, int TotalCount = 0);
}
