using System;

namespace Fast.Shared.Infrastructure.Messaging.Outbox;

internal sealed class OutboxOptions
{
    public bool Enabled { get; set; }
    public TimeSpan? StartDelay { get; set; }
    public TimeSpan? Interval { get; set; }
    public TimeSpan? InboxCleanupInterval { get; set; }
    public TimeSpan? OutboxCleanupInterval { get; set; }
}