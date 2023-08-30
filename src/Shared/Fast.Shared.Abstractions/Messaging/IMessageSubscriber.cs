using System;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Events;

namespace Fast.Shared.Abstractions.Messaging;

public interface IMessageSubscriber
{
    IMessageSubscriber Message<T>(Func<IServiceProvider, T, CancellationToken, Task> handler) where T : class, IMessage;
    IMessageSubscriber Command<T>() where T : class, ICommand;
    IMessageSubscriber Event<T>() where T : class, IEvent;
}