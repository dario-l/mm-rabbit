using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Contexts;
using Fast.Shared.Abstractions.Queries;
using Fast.Shared.Abstractions.Types;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace Fast.Shared.Infrastructure.Logging.Decorators;

[Decorator]
internal sealed class LoggingQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : class, IQuery<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _handler;
    private readonly IContextProvider _contextProvider;
    private readonly ILogger<LoggingQueryHandlerDecorator<TQuery, TResult>> _logger;

    public LoggingQueryHandlerDecorator(IQueryHandler<TQuery, TResult> handler,
        IContextProvider contextProvider, ILogger<LoggingQueryHandlerDecorator<TQuery, TResult>> logger)
    {
        _handler = handler;
        _contextProvider = contextProvider;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        var module = query.GetModuleName();
        var name = query.GetType().Name.Underscore();
        var context = _contextProvider.Current();
        var activityId = context.ActivityId;
        var traceId = context.TraceId;
        var userId = context.UserId;
        _logger.LogInformation("Handling a query: {Name} ({Module}) [Activity ID: {ActivityId}, Trace ID: {TraceId}, User ID: {UserId}]...",
            name, module, activityId, traceId, userId);
        var result = await _handler.HandleAsync(query, cancellationToken);
        _logger.LogInformation("Handled a query: {Name} ({Module}) [Activity ID: {ActivityId}, Trace ID: {TraceId}, User ID: {UserId}]",
            name, module, activityId, traceId, userId);

        return result;
    }
}