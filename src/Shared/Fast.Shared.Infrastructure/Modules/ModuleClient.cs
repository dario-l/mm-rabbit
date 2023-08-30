using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Modules;
using Fast.Shared.Infrastructure.Messaging.Contexts;

namespace Fast.Shared.Infrastructure.Modules;

internal sealed class ModuleClient : IModuleClient
{
    private readonly ConcurrentDictionary<Type, MessageAttribute> _messages = new();
    private readonly IModuleRegistry _moduleRegistry;
    private readonly IModuleSerializer _moduleSerializer;
    private readonly IMessageContextRegistry _messageContextRegistry;
    private readonly IMessageContextProvider _messageContextProvider;

    public ModuleClient(IModuleRegistry moduleRegistry, IModuleSerializer moduleSerializer,
        IMessageContextRegistry messageContextRegistry, IMessageContextProvider messageContextProvider)
    {
        _moduleRegistry = moduleRegistry;
        _moduleSerializer = moduleSerializer;
        _messageContextRegistry = messageContextRegistry;
        _messageContextProvider = messageContextProvider;
    }

    public Task SendAsync(object request, string? path = default, CancellationToken cancellationToken = default)
        => SendAsync<object>(request, path, cancellationToken);

    public async Task<TResponse?> SendAsync<TResponse>(object request, string? path = default,
        CancellationToken cancellationToken = default) where TResponse : class
    {
        var registration = _moduleRegistry.GetRequestRegistration(GetPath(request, path));
        if (registration is null)
        {
            throw new InvalidOperationException($"No action has been defined for path: '{path}'.");
        }

        var receiverRequest = TranslateType(request, registration.RequestType);
        if (receiverRequest is null)
        {
            throw new InvalidOperationException("Receiver request type couldn't be translated.");
        }
        
        var result = await registration.Action(receiverRequest, cancellationToken);

        return result is null ? default : TranslateType<TResponse>(result);
    }

    public async Task PublishAsync(object message, CancellationToken cancellationToken = default)
    {
        var topic = message.GetTopic();
        var registrations = _moduleRegistry
            .GetBroadcastRegistrations(topic)
            .Where(r => r.ReceiverType != message.GetType());

        var tasks = new List<Task>();
        foreach (var registration in registrations)
        {
            if (!_messages.TryGetValue(registration.ReceiverType, out var messageAttribute))
            {
                messageAttribute = registration.ReceiverType.GetCustomAttribute<MessageAttribute>();
                if (message is ICommand)
                {
                    messageAttribute = message.GetType().GetCustomAttribute<MessageAttribute>();
                }

                if (messageAttribute is not null)
                {
                    _messages.TryAdd(registration.ReceiverType, messageAttribute);
                }
            }

            if (messageAttribute is not null && messageAttribute.Topic != registration.Topic)
            {
                continue;
            }

            var action = registration.Action;
            var receiverMessage = TranslateType(message, registration.ReceiverType);
            if (receiverMessage is null)
            {
                throw new InvalidOperationException("Receiver message type couldn't be translated.");
            }
            
            if (message is IMessage messageData)
            {
                var messageContext = _messageContextProvider.Get(messageData);
                if (messageContext is null)
                {
                    throw new InvalidOperationException("Message context is null.");
                }
                
                _messageContextRegistry.Set((IMessage)receiverMessage, messageContext);
            }

            tasks.Add(action(receiverMessage, cancellationToken));
        }

        await Task.WhenAll(tasks);
    }

    private T? TranslateType<T>(object value)
        => _moduleSerializer.Deserialize<T>(_moduleSerializer.Serialize(value));

    private object? TranslateType(object value, Type type)
        => _moduleSerializer.Deserialize(_moduleSerializer.Serialize(value), type);

    private static string GetPath(object request, string? path)
        => string.IsNullOrWhiteSpace(path) ? request.RequestPath() : path;
}