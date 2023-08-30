using System;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Contexts;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Modules;
using Fast.Shared.Infrastructure.Messaging.Contexts;
using Fast.Shared.Infrastructure.Messaging.Dispatchers;
using Fast.Shared.Infrastructure.Messaging.Outbox;
using Humanizer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fast.Shared.Infrastructure.Messaging.Brokers;

internal sealed class InMemoryMessageBroker : IMessageBroker
{
    private readonly IModuleClient _moduleClient;
    private readonly IAsyncMessageDispatcher _asyncMessageDispatcher;
    private readonly IContextProvider _contextProvider;
    private readonly IOutboxBroker _outboxBroker;
    private readonly IMessageContextRegistry _messageContextRegistry;
    private readonly ILogger<InMemoryMessageBroker> _logger;
    private readonly bool _useAsyncDispatcher;

    public InMemoryMessageBroker(IModuleClient moduleClient, IAsyncMessageDispatcher asyncMessageDispatcher,
        IContextProvider contextProvider, IOutboxBroker outboxBroker, IMessageContextRegistry messageContextRegistry,
        IOptions<MessagingOptions> messagingOptions,ILogger<InMemoryMessageBroker> logger)
    {
        _moduleClient = moduleClient;
        _asyncMessageDispatcher = asyncMessageDispatcher;
        _contextProvider = contextProvider;
        _outboxBroker = outboxBroker;
        _messageContextRegistry = messageContextRegistry;
        _useAsyncDispatcher = messagingOptions.Value.UseAsyncDispatcher;
        _logger = logger;
    }

    public  async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : class, IMessage
    {        
        var context = _contextProvider.Current();
        var activityId = context.ActivityId;
        var traceId = context.TraceId;
        var userId = context.UserId;

        var messageId = Guid.NewGuid();
        var messageContext = new MessageContext(messageId, context);
        _messageContextRegistry.Set(message, messageContext);
        var module = message.GetModuleName();
        var name = message.GetType().Name.Underscore();
        _logger.LogInformation("Sending a message: {Name} ({Module}) [Activity ID: {ActivityId}, Message ID: {MessageId}, Trace ID: {TraceId}, User ID: {UserId}]...",
            name, module, activityId, messageId, traceId, userId);

        if (_outboxBroker.Enabled(module))
        {
            await _outboxBroker.SendAsync(message);
            return;
        }

        if (_useAsyncDispatcher)
        {
            await _asyncMessageDispatcher.PublishAsync(message, cancellationToken);
            return;
        }

        await _moduleClient.PublishAsync(message, cancellationToken);
    }
}