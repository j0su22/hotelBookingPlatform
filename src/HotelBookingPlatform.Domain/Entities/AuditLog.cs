namespace HotelBookingPlatform.Domain.Entities;

public sealed class AuditLog
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string EntityName { get; private set; } = default!;
    public string EntityId { get; private set; } = default!;
    public string Action { get; private set; } = default!;
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public DateTime PerformedAt { get; private set; } = DateTime.UtcNow;
    public string? PerformedBy { get; private set; }

    public static AuditLog Create(
        string entityName,
        string entityId,
        string action,
        string? oldValues = null,
        string? newValues = null,
        string? performedBy = null)
    {
        return new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            OldValues = oldValues,
            NewValues = newValues,
            PerformedBy = performedBy
        };
    }
}
