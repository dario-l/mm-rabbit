using System;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fast.Shared.Infrastructure.Messaging.Outbox;

internal class OutboxBroker : IOutboxBroker
{
    private readonly IServiceProvider _serviceProvider;
    private readonly OutboxTypeRegistry _registry;
    private readonly bool _enabled;

    public OutboxBroker(IServiceProvider serviceProvider, OutboxTypeRegistry registry, IOptions<OutboxOptions> options)
    {
        _serviceProvider = serviceProvider;
        _registry = registry;
        _enabled = options.Value.Enabled;
    }

    public bool Enabled(string module) => _enabled && _registry.Resolve(module) is not null;
    
    public async Task SendAsync(params IMessage[] messages)
    {
        var message = messages[0]; // Not possible to send messages from different modules at once
        var outboxType = _registry.Resolve(message);
        if (outboxType is null)
        {
            throw new InvalidOperationException($"Outbox is not registered for module: '{message.GetModuleName()}'.");
        }

        using var scope = _serviceProvider.CreateScope();
        var outbox = (IOutbox)scope.ServiceProvider.GetRequiredService(outboxType);
        await outbox.SaveAsync(messages);
    }
}