using System.Data;
using Dapper;

namespace HotelBookingPlatform.Infrastructure.Persistence;

/// <summary>
/// Dapper does not natively support DateOnly (.NET 6+).
/// This handler converts between SQL Server date columns and DateOnly.
/// </summary>
public sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override DateOnly Parse(object value) =>
        DateOnly.FromDateTime((DateTime)value);

    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    }
}
