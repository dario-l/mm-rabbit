using System;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Contexts;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Types;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace Fast.Shared.Infrastructure.Logging.Decorators;

[Decorator]
internal sealed class LoggingCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
{
    private readonly ICommandHandler<T> _handler;
    private readonly IMessageContextProvider _messageContextProvider;
    private readonly IContextProvider _contextProvider;
    private readonly ILogger<LoggingCommandHandlerDecorator<T>> _logger;

    public LoggingCommandHandlerDecorator(ICommandHandler<T> handler, IMessageContextProvider messageContextProvider,
        IContextProvider contextProvider, ILogger<LoggingCommandHandlerDecorator<T>> logger)
    {
        _handler = handler;
        _messageContextProvider = messageContextProvider;
        _contextProvider = contextProvider;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(T command, CancellationToken cancellationToken = default)
    {
        var module = command.GetModuleName();
        var name = command.GetType().Name.Underscore();
        var messageContext = _messageContextProvider.Get(command);
        var context = _contextProvider.Current();
        var activityId = context.ActivityId;
        var traceId = context.TraceId;
        var userId = context.UserId;
        var messageId = messageContext?.MessageId;
        _logger.LogInformation("Handling a command: {Name} ({Module}) [Activity ID: {ActivityId}, Message ID: {MessageId}, Trace ID: {TraceId}, User ID: {UserId}]...",
            name, module, activityId, messageId, traceId, userId);
        await _handler.HandleAsync(command, cancellationToken);
        _logger.LogInformation("Handled a command: {Name} ({Module}) [Activity ID: {ActivityId}, Message ID: {MessageId}, Trace ID: {TraceId}, User ID: {UserId}].",
            name, module, activityId, messageId, traceId, userId);
    }
}