using System;
using Fast.Shared.Abstractions.Contexts;

namespace Fast.Shared.Abstractions.Messaging;

public interface IMessageContext
{
    public Guid MessageId { get; }
    public IContext Context { get; }
}