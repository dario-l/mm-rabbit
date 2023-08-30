using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fast.Shared.Abstractions.Modules;

public interface IModuleEndpointsBuilder
{
    IModuleEndpointsBuilder Map<TRequest>(
        Func<TRequest, IServiceProvider, CancellationToken, Task> action,
        string? path = default)
        where TRequest : class;
    
    IModuleEndpointsBuilder Map<TRequest, TResponse>(
        Func<TRequest, IServiceProvider, CancellationToken, Task<TResponse>> action,
        string? path = default)
        where TRequest : class where TResponse : class;
}