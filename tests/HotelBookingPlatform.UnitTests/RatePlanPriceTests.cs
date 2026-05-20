using FluentAssertions;
using HotelBookingPlatform.Domain.Entities;

namespace HotelBookingPlatform.UnitTests;

public class RatePlanPriceTests
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    [Fact]
    public void CalculatePrice_ValidRange_ShouldReturnCorrectTotal()
    {
        var plan = RatePlan.Create(Guid.NewGuid(), "Standard", 100m, Today, Today.AddDays(30));
        var result = plan.CalculatePrice(Today.AddDays(1), Today.AddDays(4));
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(300m); // 3 nights * 100
    }

    [Fact]
    public void CalculatePrice_OutsideValidRange_ShouldFail()
    {
        var plan = RatePlan.Create(Guid.NewGuid(), "Standard", 100m, Today, Today.AddDays(5));
        var result = plan.CalculatePrice(Today.AddDays(1), Today.AddDays(10));
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("RATE_PLAN_DATE_OUT_OF_RANGE");
    }

    [Fact]
    public void CalculatePrice_InactivePlan_ShouldFail()
    {
        var plan = RatePlan.Create(Guid.NewGuid(), "Standard", 100m, Today, Today.AddDays(30));
        plan.Deactivate();
        var result = plan.CalculatePrice(Today.AddDays(1), Today.AddDays(3));
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("RATE_PLAN_INACTIVE");
    }

    [Fact]
    public void CalculatePrice_SingleNight_ShouldReturnPricePerNight()
    {
        var plan = RatePlan.Create(Guid.NewGuid(), "Standard", 150m, Today, Today.AddDays(10));
        var result = plan.CalculatePrice(Today.AddDays(1), Today.AddDays(2));
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(150m);
    }
}
