using System;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Shared.Infrastructure.Modules;

internal sealed class ModuleEndpointsBuilder : IModuleEndpointsBuilder
{
    private readonly IModuleRegistry _moduleRegistry;
    private readonly IServiceProvider _serviceProvider;

    public ModuleEndpointsBuilder(IModuleRegistry moduleRegistry, IServiceProvider serviceProvider)
    {
        _moduleRegistry = moduleRegistry;
        _serviceProvider = serviceProvider;
    }

    public IModuleEndpointsBuilder Map<TRequest>(
        Func<TRequest, IServiceProvider, CancellationToken, Task> action,
        string? path = default) where TRequest : class
    {
        _moduleRegistry.AddRequestAction(GetPath<TRequest>(path), typeof(TRequest), typeof(object),
            async (request, cancellationToken) =>
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                await action((TRequest) request, scope.ServiceProvider, cancellationToken);
                return default;
            });

        return this;
    }

    public IModuleEndpointsBuilder Map<TRequest, TResponse>(
        Func<TRequest, IServiceProvider, CancellationToken, Task<TResponse>> action,
        string? path = default)
        where TRequest : class where TResponse : class
    {
        _moduleRegistry.AddRequestAction(GetPath<TRequest>(path), typeof(TRequest), typeof(TResponse),
            async (request, cancellationToken) =>
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                return await action((TRequest) request, scope.ServiceProvider, cancellationToken);
            });

        return this;
    }

    private static string GetPath<TRequest>(string? path) where TRequest : class
        => string.IsNullOrWhiteSpace(path) ? To.RequestPath<TRequest>() : path;
}