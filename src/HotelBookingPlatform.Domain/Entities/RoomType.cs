using HotelBookingPlatform.Domain.Common;

namespace HotelBookingPlatform.Domain.Entities;

public sealed class RoomType : AggregateRoot
{
    private RoomType() { }

    public Guid HotelId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int MaxCapacity { get; private set; }
    public decimal BasePrice { get; private set; }
    public bool IsActive { get; private set; } = true;
    public byte[] RowVersion { get; private set; } = default!;

    public Hotel Hotel { get; private set; } = default!;

    private readonly List<RatePlan> _ratePlans = [];
    public IReadOnlyCollection<RatePlan> RatePlans => _ratePlans.AsReadOnly();

    private readonly List<RoomInventory> _roomInventories = [];
    public IReadOnlyCollection<RoomInventory> RoomInventories => _roomInventories.AsReadOnly();

    public static RoomType Create(Guid hotelId, string name, string description, int maxCapacity, decimal basePrice)
    {
        return new RoomType
        {
            HotelId = hotelId,
            Name = name,
            Description = description,
            MaxCapacity = maxCapacity,
            BasePrice = basePrice
        };
    }

    public void Update(string name, string description, int maxCapacity, decimal basePrice)
    {
        Name = name;
        Description = description;
        MaxCapacity = maxCapacity;
        BasePrice = basePrice;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
