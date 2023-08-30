using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Dispatchers;
using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Infrastructure.Messaging.RabbitMQ.Internals;
using Microsoft.Extensions.DependencyInjection;
using IMessage = Fast.Shared.Abstractions.Messaging.IMessage;

namespace Fast.Shared.Infrastructure.Messaging.RabbitMQ;

internal sealed class RabbitMQMessageSubscriber : IMessageSubscriber
{
    private readonly IMessageHandler _messageHandler;
    private readonly IMessageTypeRegistry _messageTypeRegistry;
    private readonly IBus _bus;

    public RabbitMQMessageSubscriber(IMessageHandler messageHandler, IMessageTypeRegistry messageTypeRegistry, IBus bus)
    {
        _messageHandler = messageHandler;
        _messageTypeRegistry = messageTypeRegistry;
        _bus = bus;
    }

    public IMessageSubscriber Command<T>() where T : class, ICommand
        => Message<T>((serviceProvider, command, cancellationToken) =>
            serviceProvider.GetRequiredService<IDispatcher>().SendAsync(command, cancellationToken));

    public IMessageSubscriber Event<T>() where T : class, IEvent
        => Message<T>((serviceProvider, @event, cancellationToken) =>
            serviceProvider.GetRequiredService<IDispatcher>().PublishAsync(@event, cancellationToken));

    public IMessageSubscriber Message<T>(Func<IServiceProvider, T, CancellationToken, Task> handler)
        where T : class, IMessage
    {
        _messageTypeRegistry.Register<T>();
        var messageAttribute = typeof(T).GetCustomAttribute<MessageAttribute>() ?? new MessageAttribute();

        _bus.PubSub.SubscribeAsync<T>(messageAttribute.SubscriptionId,
            (message, cancellationToken) => _messageHandler.HandleAsync(handler, message, cancellationToken),
            configuration =>
            {
                if (!string.IsNullOrWhiteSpace(messageAttribute.Topic))
                {
                    configuration.WithTopic(messageAttribute.Topic);
                }

                if (!string.IsNullOrWhiteSpace(messageAttribute.Queue))
                {
                    configuration.WithQueueName(messageAttribute.Queue);
                }
            });

        return this;
    }
}