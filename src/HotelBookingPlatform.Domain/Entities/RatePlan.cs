using HotelBookingPlatform.Domain.Common;

namespace HotelBookingPlatform.Domain.Entities;

public sealed class RatePlan : Entity
{
    private RatePlan() { }

    public Guid RoomTypeId { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal PricePerNight { get; private set; }
    public DateOnly ValidFrom { get; private set; }
    public DateOnly ValidTo { get; private set; }
    public bool IsActive { get; private set; } = true;

    public RoomType RoomType { get; private set; } = default!;

    public static RatePlan Create(Guid roomTypeId, string name, decimal pricePerNight, DateOnly validFrom, DateOnly validTo)
    {
        return new RatePlan
        {
            RoomTypeId = roomTypeId,
            Name = name,
            PricePerNight = pricePerNight,
            ValidFrom = validFrom,
            ValidTo = validTo
        };
    }

    public void Update(string name, decimal pricePerNight, DateOnly validFrom, DateOnly validTo)
    {
        Name = name;
        PricePerNight = pricePerNight;
        ValidFrom = validFrom;
        ValidTo = validTo;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public Result<decimal> CalculatePrice(DateOnly checkIn, DateOnly checkOut)
    {
        if (!IsActive)
            return Result.Failure<decimal>("El plan de tarifas no está activo.", "RATE_PLAN_INACTIVE");

        if (checkIn < ValidFrom || checkOut > ValidTo.AddDays(1))
            return Result.Failure<decimal>("Las fechas están fuera del rango de validez del plan.", "RATE_PLAN_DATE_OUT_OF_RANGE");

        int nights = checkOut.DayNumber - checkIn.DayNumber;
        return Result.Success(PricePerNight * nights);
    }
}
