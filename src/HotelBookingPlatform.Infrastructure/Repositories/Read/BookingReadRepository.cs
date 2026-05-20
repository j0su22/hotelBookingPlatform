using Dapper;
using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using HotelBookingPlatform.Domain.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HotelBookingPlatform.Infrastructure.Repositories.Read;

public sealed class BookingReadRepository(IConfiguration configuration) : IBookingReadRepository
{
    private SqlConnection CreateConnection() =>
        new(configuration.GetConnectionString("Default"));

    public async Task<PagedResult<BookingDto>> GetPagedAsync(BookingFilters filters, PagedRequest request, CancellationToken cancellationToken = default)
    {
        var allowedSortColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "CheckIn", "CheckOut", "TotalPrice", "Status", "CreatedAt" };
        var sortBy = allowedSortColumns.Contains(request.SortBy) ? $"b.{request.SortBy}" : "b.CreatedAt";
        var sortDir = request.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";
        var skip = (request.PageNumber - 1) * request.PageSize;

        var whereConditions = new List<string>();
        if (filters.Status is not null)
            whereConditions.Add("bs.Name = @Status");
        if (filters.GuestEmail is not null)
            whereConditions.Add("g.Email = @GuestEmail");
        if (filters.HotelId.HasValue)
            whereConditions.Add("h.Id = @HotelId");
        if (filters.CheckInFrom.HasValue)
            whereConditions.Add("b.CheckIn >= @CheckInFrom");
        if (filters.CheckInTo.HasValue)
            whereConditions.Add("b.CheckIn <= @CheckInTo");

        var whereClause = whereConditions.Count > 0 ? "WHERE " + string.Join(" AND ", whereConditions) : "";

        var sql = $"""
            SELECT
                b.Id,
                b.ConfirmationNumber,
                g.FirstName + ' ' + g.LastName AS GuestName,
                g.Email AS GuestEmail,
                h.Name AS HotelName,
                rt.Name AS RoomTypeName,
                b.CheckIn,
                b.CheckOut,
                DATEDIFF(day, b.CheckIn, b.CheckOut) AS Nights,
                b.TotalPrice,
                CASE b.Status WHEN 1 THEN 'Pending' WHEN 2 THEN 'Confirmed' WHEN 3 THEN 'Cancelled' ELSE 'Unknown' END AS Status,
                b.CreatedAt,
                COUNT(*) OVER() AS TotalCount
            FROM Bookings b
            INNER JOIN Guests g ON g.Id = b.GuestId
            INNER JOIN RoomTypes rt ON rt.Id = b.RoomTypeId
            INNER JOIN Hotels h ON h.Id = rt.HotelId
            LEFT JOIN (VALUES (1,'Pending'),(2,'Confirmed'),(3,'Cancelled')) AS bs(Id,Name) ON bs.Id = b.Status
            {whereClause}
            ORDER BY {sortBy} {sortDir}
            OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

        await using var conn = CreateConnection();
        var rows = await conn.QueryAsync<BookingRow>(sql, new
        {
            Status = filters.Status,
            GuestEmail = filters.GuestEmail?.ToLowerInvariant(),
            HotelId = filters.HotelId,
            CheckInFrom = filters.CheckInFrom,
            CheckInTo = filters.CheckInTo,
            Skip = skip,
            PageSize = request.PageSize
        });

        var list = rows.ToList();
        var totalRecords = list.Count > 0 ? list[0].TotalCount : 0;
        var data = list.Select(r => new BookingDto(r.Id, r.ConfirmationNumber, r.GuestName, r.GuestEmail,
            r.HotelName, r.RoomTypeName, r.CheckIn, r.CheckOut, r.Nights, r.TotalPrice, r.Status, r.CreatedAt)).ToList();

        return new PagedResult<BookingDto>
        {
            Data = data,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords
        };
    }

    public async Task<BookingDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                b.Id,
                b.ConfirmationNumber,
                g.Id AS GuestId,
                g.FirstName AS GuestFirstName,
                g.LastName AS GuestLastName,
                g.Email AS GuestEmail,
                g.PhoneNumber AS GuestPhone,
                h.Id AS HotelId,
                h.Name AS HotelName,
                rt.Id AS RoomTypeId,
                rt.Name AS RoomTypeName,
                b.CheckIn,
                b.CheckOut,
                DATEDIFF(day, b.CheckIn, b.CheckOut) AS Nights,
                b.GuestCount,
                b.TotalPrice,
                CASE b.Status WHEN 1 THEN 'Pending' WHEN 2 THEN 'Confirmed' WHEN 3 THEN 'Cancelled' ELSE 'Unknown' END AS Status,
                b.CreatedAt,
                b.UpdatedAt
            FROM Bookings b
            INNER JOIN Guests g ON g.Id = b.GuestId
            INNER JOIN RoomTypes rt ON rt.Id = b.RoomTypeId
            INNER JOIN Hotels h ON h.Id = rt.HotelId
            WHERE b.Id = @Id
            """;

        await using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<BookingDetailDto>(sql, new { Id = id });
    }

    private sealed record BookingRow(
        Guid Id, string ConfirmationNumber, string GuestName, string GuestEmail,
        string HotelName, string RoomTypeName, DateOnly CheckIn, DateOnly CheckOut,
        int Nights, decimal TotalPrice, string Status, DateTime CreatedAt, int TotalCount = 0);
}
