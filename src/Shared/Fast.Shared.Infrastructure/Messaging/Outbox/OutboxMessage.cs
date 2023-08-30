using System;

namespace Fast.Shared.Infrastructure.Messaging.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; set; }
    public string ActivityId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
}