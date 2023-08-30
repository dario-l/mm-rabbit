using System;

namespace Fast.Shared.Infrastructure.Messaging.Inbox;

public sealed class InboxMessage
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}