using FluentAssertions;
using HotelBookingPlatform.Domain.Entities;

namespace HotelBookingPlatform.UnitTests;

public class RoomInventoryTests
{
    [Fact]
    public void Decrease_WithAvailableRooms_ShouldSucceed()
    {
        var inventory = RoomInventory.Create(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today), totalRooms: 5);
        var result = inventory.Decrease();
        result.IsSuccess.Should().BeTrue();
        inventory.AvailableRooms.Should().Be(4);
    }

    [Fact]
    public void Decrease_WhenNoRoomsAvailable_ShouldFail()
    {
        var inventory = RoomInventory.Create(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today), totalRooms: 1);
        inventory.Decrease(); // use the last one
        var result = inventory.Decrease();
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("INVENTORY_INSUFFICIENT");
    }

    [Fact]
    public void Increase_ShouldNotExceedTotalRooms()
    {
        var inventory = RoomInventory.Create(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today), totalRooms: 5);
        inventory.Decrease();
        inventory.Increase();
        inventory.Increase(); // Should cap at TotalRooms
        inventory.AvailableRooms.Should().Be(5);
    }
}
