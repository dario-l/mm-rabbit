using System.Threading;
using System.Threading.Tasks;

namespace Fast.Shared.Abstractions.Messaging;

public interface IMessageBroker
{
    Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : class, IMessage;
}