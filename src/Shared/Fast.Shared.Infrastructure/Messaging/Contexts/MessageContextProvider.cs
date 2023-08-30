using Fast.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Caching.Memory;

namespace Fast.Shared.Infrastructure.Messaging.Contexts;

internal sealed class MessageContextProvider : IMessageContextProvider
{
    private readonly IMemoryCache _cache;

    public MessageContextProvider(IMemoryCache cache)
    {
        _cache = cache;
    }

    public IMessageContext? Get(IMessage message) => _cache.Get<IMessageContext>(message);
}