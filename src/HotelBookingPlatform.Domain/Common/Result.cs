namespace HotelBookingPlatform.Domain.Common;

public class Result
{
    protected Result(bool isSuccess, string? errorMessage, string? errorCode)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? ErrorMessage { get; }
    public string? ErrorCode { get; }

    public static Result Success() => new(true, null, null);

    public static Result Failure(string errorMessage, string? errorCode = null) =>
        new(false, errorMessage, errorCode);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(string errorMessage, string? errorCode = null) =>
        Result<T>.Failure(errorMessage, errorCode);
}
