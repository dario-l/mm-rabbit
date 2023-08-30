using System;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Shared.Infrastructure.Messaging.RabbitMQ.Internals;

public interface IMessageHandler
{
    Task HandleAsync<T>(Func<IServiceProvider, T, CancellationToken, Task> handler, T message,
        CancellationToken cancellationToken = default) where T : IMessage;
}