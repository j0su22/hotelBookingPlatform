namespace HotelBookingPlatform.Domain.Common;

public sealed class Result<T> : Result
{
    private Result(bool isSuccess, T? value, string? errorMessage, string? errorCode)
        : base(isSuccess, errorMessage, errorCode)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(true, value, null, null);

    public new static Result<T> Failure(string errorMessage, string? errorCode = null) =>
        new(false, default, errorMessage, errorCode);
}
