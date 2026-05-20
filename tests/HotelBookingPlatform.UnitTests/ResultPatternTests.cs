using FluentAssertions;
using HotelBookingPlatform.Domain.Common;

namespace HotelBookingPlatform.UnitTests;

public class ResultPatternTests
{
    [Fact]
    public void Result_Success_ShouldBeSuccessful()
    {
        var result = Result.Success();
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Result_Failure_ShouldContainError()
    {
        var result = Result.Failure("Error message", "ERROR_CODE");
        result.IsFailure.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Error message");
        result.ErrorCode.Should().Be("ERROR_CODE");
    }

    [Fact]
    public void ResultT_Success_ShouldContainValue()
    {
        var result = Result.Success(42);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void ResultT_Failure_ShouldNotContainValue()
    {
        var result = Result.Failure<int>("Failed", "CODE");
        result.IsFailure.Should().BeTrue();
        result.Value.Should().Be(default);
        result.ErrorCode.Should().Be("CODE");
    }
}
