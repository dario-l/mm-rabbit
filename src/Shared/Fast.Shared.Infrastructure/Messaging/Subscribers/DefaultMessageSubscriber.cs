using System;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Shared.Infrastructure.Messaging.Subscribers;

internal sealed class DefaultMessageSubscriber : IMessageSubscriber
{
    public IMessageSubscriber Message<T>(Func<IServiceProvider, T, CancellationToken, Task> handler) where T : class, IMessage => this;

    public IMessageSubscriber Command<T>() where T : class, ICommand => this;

    public IMessageSubscriber Event<T>() where T : class, IEvent => this;
}