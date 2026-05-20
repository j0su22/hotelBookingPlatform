using HotelBookingPlatform.Domain.Common;

namespace HotelBookingPlatform.Domain.Entities;

public sealed class Hotel : AggregateRoot
{
    private Hotel() { }

    public string Name { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string Country { get; private set; } = default!;
    public int StarRating { get; private set; }
    public bool IsActive { get; private set; } = true;
    public byte[] RowVersion { get; private set; } = default!;

    private readonly List<RoomType> _roomTypes = [];
    public IReadOnlyCollection<RoomType> RoomTypes => _roomTypes.AsReadOnly();

    public static Hotel Create(string name, string address, string city, string country, int starRating)
    {
        return new Hotel
        {
            Name = name,
            Address = address,
            City = city,
            Country = country,
            StarRating = starRating
        };
    }

    public void Update(string name, string address, string city, string country, int starRating)
    {
        Name = name;
        Address = address;
        City = city;
        Country = country;
        StarRating = starRating;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
