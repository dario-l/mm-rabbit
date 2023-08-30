using System;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Contexts;
using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Types;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace Fast.Shared.Infrastructure.Logging.Decorators;

[Decorator]
internal sealed class LoggingEventHandlerDecorator<T> : IEventHandler<T> where T : class, IEvent
{
    private readonly IEventHandler<T> _handler;
    private readonly IMessageContextProvider _messageContextProvider;
    private readonly IContextProvider _contextProvider;
    private readonly ILogger<LoggingEventHandlerDecorator<T>> _logger;

    public LoggingEventHandlerDecorator(IEventHandler<T> handler, IMessageContextProvider messageContextProvider,
        IContextProvider contextProvider, ILogger<LoggingEventHandlerDecorator<T>> logger)
    {
        _handler = handler;
        _messageContextProvider = messageContextProvider;
        _contextProvider = contextProvider;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        var module = @event.GetModuleName();
        var name = @event.GetType().Name.Underscore();
        var messageContext = _messageContextProvider.Get(@event);
        var context = _contextProvider.Current();
        var activityId = context.ActivityId;
        var traceId = context.TraceId;
        var userId = context.UserId;
        var messageId = messageContext?.MessageId;
        _logger.LogInformation("Handling an event: {Name} ({Module}) [Activity ID: {ActivityId}, Message ID: {MessageId}, Trace ID: {TraceId}, User ID: {UserId}]...",
            name, module, activityId, messageId, traceId, userId);
        await _handler.HandleAsync(@event, cancellationToken);
        _logger.LogInformation("Handled an event: {Name} ({Module}) [Activity ID: {ActivityId}, Message ID: {MessageId}, Trace ID: {TraceId}, User ID: {UserId}].",
            name, module, activityId, messageId, traceId, userId);
    }
}