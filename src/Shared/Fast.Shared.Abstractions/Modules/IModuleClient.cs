using System.Threading;
using System.Threading.Tasks;

namespace Fast.Shared.Abstractions.Modules;

public interface IModuleClient
{
    Task SendAsync(object request, string? path = default, CancellationToken cancellationToken = default);

    Task<TResponse?> SendAsync<TResponse>(object request, string? path = default,
        CancellationToken cancellationToken = default) where TResponse : class;

    Task PublishAsync(object message, CancellationToken cancellationToken = default);
}