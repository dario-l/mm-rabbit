using System;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Fast.Shared.Infrastructure.Messaging.RabbitMQ.Internals;

internal sealed class MessageHandler : IMessageHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(IServiceProvider serviceProvider, ILogger<MessageHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task HandleAsync<T>(Func<IServiceProvider, T, CancellationToken, Task> handler, T message,
        CancellationToken cancellationToken = default) where T : IMessage
    {
        try
        {
            await handler(_serviceProvider, message, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            throw;
        }
    }
}