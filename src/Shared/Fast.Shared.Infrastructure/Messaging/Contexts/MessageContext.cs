using System;
using Fast.Shared.Abstractions.Contexts;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Shared.Infrastructure.Messaging.Contexts;

internal sealed class MessageContext : IMessageContext
{
    public Guid MessageId { get; }
    public IContext Context { get; }

    public MessageContext(Guid messageId, IContext context)
    {
        MessageId = messageId;
        Context = context;
    }
}