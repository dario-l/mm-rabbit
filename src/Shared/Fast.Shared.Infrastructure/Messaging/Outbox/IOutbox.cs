using System;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Shared.Infrastructure.Messaging.Outbox;

internal interface IOutbox
{
    bool Enabled { get; }
    Task SaveAsync(params IMessage[] messages);
    Task PublishUnsentAsync();
    Task CleanupAsync(DateTime? to = null);
}