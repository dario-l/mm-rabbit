using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Fast.Shared.Abstractions.Messaging;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace Fast.Shared.Infrastructure.Messaging.RabbitMQ;

internal sealed class RabbitMQMessageBroker : IMessageBroker
{
    private readonly ConcurrentDictionary<Type, string> _names = new();
    private readonly IBus _bus;
    private readonly ILogger<RabbitMQMessageBroker> _logger;

    public RabbitMQMessageBroker(IBus bus, ILogger<RabbitMQMessageBroker> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : class, Abstractions.Messaging.IMessage
    {
        var messageName = _names.GetOrAdd(typeof(T), typeof(T).Name.Underscore());
        _logger.LogInformation("Sending a message: {MessageName}...", messageName);
        await _bus.PubSub.PublishAsync(message, cancellationToken);
    }
}