namespace HotelBookingPlatform.Domain.Entities;

public sealed class IdempotencyRecord
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Key { get; private set; } = default!;
    public string? RequestHash { get; private set; }
    public int ResponseStatus { get; private set; }
    public string ResponseBody { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; private set; }

    public static IdempotencyRecord Create(
        string key,
        int responseStatus,
        string responseBody,
        string? requestHash = null,
        TimeSpan? ttl = null)
    {
        return new IdempotencyRecord
        {
            Key = key,
            RequestHash = requestHash,
            ResponseStatus = responseStatus,
            ResponseBody = responseBody,
            ExpiresAt = DateTime.UtcNow.Add(ttl ?? TimeSpan.FromHours(24))
        };
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
}
