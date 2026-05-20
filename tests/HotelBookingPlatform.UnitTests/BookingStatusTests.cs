using FluentAssertions;
using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Enums;

namespace HotelBookingPlatform.UnitTests;

public class BookingStatusTests
{
    private static Booking CreatePendingBooking() =>
        Booking.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            200m, Guid.NewGuid().ToString(), 2);

    [Fact]
    public void Confirm_FromPending_ShouldSucceed()
    {
        var booking = CreatePendingBooking();
        var result = booking.Confirm();
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Confirmed);
    }

    [Fact]
    public void Confirm_FromCancelled_ShouldFail()
    {
        var booking = CreatePendingBooking();
        booking.Cancel();
        var result = booking.Confirm();
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("BOOKING_INVALID_STATE");
    }

    [Fact]
    public void Cancel_FromPending_ShouldSucceed()
    {
        var booking = CreatePendingBooking();
        var result = booking.Cancel();
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Cancelled);
    }

    [Fact]
    public void Cancel_FromConfirmed_ShouldSucceed()
    {
        var booking = CreatePendingBooking();
        booking.Confirm();
        var result = booking.Cancel();
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Cancelled);
    }

    [Fact]
    public void Cancel_AlreadyCancelled_ShouldFail()
    {
        var booking = CreatePendingBooking();
        booking.Cancel();
        var result = booking.Cancel();
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("BOOKING_ALREADY_CANCELLED");
    }

    [Fact]
    public void Create_ShouldRaiseDomainEvent()
    {
        var booking = CreatePendingBooking();
        booking.DomainEvents.Should().HaveCount(1);
        booking.DomainEvents.First().GetType().Name.Should().Be("BookingCreatedEvent");
    }
}
