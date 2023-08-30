using System;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Types;
using Fast.Shared.Infrastructure.Messaging.Outbox;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fast.Shared.Infrastructure.Messaging.Inbox;

[Decorator]
internal sealed class InboxEventHandlerDecorator<T> : IEventHandler<T> where T : class, IEvent
{
    private readonly IEventHandler<T> _handler;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageContextProvider _messageContextProvider;
    private readonly InboxTypeRegistry _inboxTypeRegistry;
    private readonly bool _enabled;

    public InboxEventHandlerDecorator(IEventHandler<T> handler, IServiceProvider serviceProvider,
        IMessageContextProvider messageContextProvider, InboxTypeRegistry inboxTypeRegistry,
        IOptions<OutboxOptions> options)
    {
        _handler = handler;
        _serviceProvider = serviceProvider;
        _messageContextProvider = messageContextProvider;
        _inboxTypeRegistry = inboxTypeRegistry;
        _enabled = options.Value.Enabled;
    }

    public async Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        if (_enabled)
        {
            var inboxType = _inboxTypeRegistry.Resolve<T>();
            if (inboxType is null)
            {
                await _handler.HandleAsync(@event, cancellationToken);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var inbox = (IInbox) _serviceProvider.GetRequiredService(inboxType);
            var messageContext = _messageContextProvider.Get(@event);
            if (messageContext is null)
            {
                throw new InvalidOperationException("Message context is null.");
            }
            
            var name = @event.GetType().Name.Underscore();
            await inbox.HandleAsync(messageContext.MessageId, name, () => _handler.HandleAsync(@event, cancellationToken));
            return;
        }

        await _handler.HandleAsync(@event, cancellationToken);
    }
}