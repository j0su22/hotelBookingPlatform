using HotelBookingPlatform.Domain.Common;

namespace HotelBookingPlatform.Domain.Entities;

public sealed class RoomInventory : Entity
{
    private RoomInventory() { }

    public Guid RoomTypeId { get; private set; }
    public DateOnly Date { get; private set; }
    public int TotalRooms { get; private set; }
    public int AvailableRooms { get; private set; }
    public byte[] RowVersion { get; private set; } = default!;

    public RoomType RoomType { get; private set; } = default!;

    public static RoomInventory Create(Guid roomTypeId, DateOnly date, int totalRooms)
    {
        return new RoomInventory
        {
            RoomTypeId = roomTypeId,
            Date = date,
            TotalRooms = totalRooms,
            AvailableRooms = totalRooms
        };
    }

    public Result Decrease(int count = 1)
    {
        if (AvailableRooms < count)
            return Result.Failure("No hay habitaciones disponibles para esta fecha.", "INVENTORY_INSUFFICIENT");

        AvailableRooms -= count;
        SetUpdatedAt();
        return Result.Success();
    }

    public void Increase(int count = 1)
    {
        AvailableRooms = Math.Min(TotalRooms, AvailableRooms + count);
        SetUpdatedAt();
    }

    /// <summary>
    /// Updates the total room count. Available rooms are adjusted:
    /// increasing total frees up rooms; decreasing respects already-booked rooms.
    /// </summary>
    public void UpdateTotalRooms(int newTotal)
    {
        var booked = TotalRooms - AvailableRooms;        // rooms already reserved
        TotalRooms = newTotal;
        AvailableRooms = Math.Max(0, newTotal - booked); // respect existing bookings
        SetUpdatedAt();
    }
}
