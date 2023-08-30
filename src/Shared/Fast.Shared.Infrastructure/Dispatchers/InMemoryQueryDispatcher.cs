using System;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Shared.Infrastructure.Dispatchers;

internal sealed class InMemoryQueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryQueryDispatcher(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync));
        if (method is null)
        {
            throw new InvalidOperationException($"Query handler for '{typeof(TResult).Name}' is invalid.");
        }

#pragma warning disable CS8602
#pragma warning disable CS8600
        return await (Task<TResult>)method.Invoke(handler, new object[] {query, cancellationToken});
#pragma warning restore CS8600
#pragma warning restore CS8602
    }
}