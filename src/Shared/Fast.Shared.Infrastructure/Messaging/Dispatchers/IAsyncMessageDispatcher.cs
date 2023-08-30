using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Shared.Infrastructure.Messaging.Dispatchers;

internal interface IAsyncMessageDispatcher
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, IMessage;
}