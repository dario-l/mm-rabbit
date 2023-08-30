using System;
using Fast.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Caching.Memory;

namespace Fast.Shared.Infrastructure.Messaging.Contexts;

internal sealed class MessageContextRegistry : IMessageContextRegistry
{
    private readonly IMemoryCache _cache;

    public MessageContextRegistry(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void Set(IMessage message, IMessageContext context)
        => _cache.Set(message, context, TimeSpan.FromMinutes(1));
}