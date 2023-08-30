using System.Threading.Tasks;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Shared.Infrastructure.Messaging.Outbox;

internal interface IOutboxBroker
{
    bool Enabled(string module);
    Task SendAsync(params IMessage[] messages);
}