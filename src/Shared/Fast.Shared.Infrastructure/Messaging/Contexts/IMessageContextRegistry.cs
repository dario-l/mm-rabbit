using Fast.Shared.Abstractions.Messaging;

namespace Fast.Shared.Infrastructure.Messaging.Contexts;

internal interface IMessageContextRegistry
{
    void Set(IMessage message, IMessageContext context);
}