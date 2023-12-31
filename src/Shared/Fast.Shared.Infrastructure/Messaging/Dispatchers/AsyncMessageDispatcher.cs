﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Shared.Infrastructure.Messaging.Dispatchers;

internal sealed class AsyncMessageDispatcher : IAsyncMessageDispatcher
{
    private readonly IMessageChannel _channel;
    private readonly IMessageContextProvider _messageContextProvider;

    public AsyncMessageDispatcher(IMessageChannel channel, IMessageContextProvider messageContextProvider)
    {
        _channel = channel;
        _messageContextProvider = messageContextProvider;
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var messageContext = _messageContextProvider.Get(message);
        if (messageContext is null)
        {
            throw new InvalidOperationException("Message context is null.");
        }
        
        await _channel.Writer.WriteAsync(new MessageEnvelope(message, messageContext), cancellationToken);
    }
}