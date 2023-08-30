using System;
using System.Collections.Concurrent;
using EasyNetQ;

namespace Fast.Shared.Infrastructure.Messaging.RabbitMQ.Internals;

internal sealed class CustomMessageSerializationStrategy : IMessageSerializationStrategy
{
    private const string ActivityIdKey = "activity-id";
    private const string CausationIdKey = "causation-id";
    private const string TraceIdKey = "trace-id";
    private const string UserIdKey = "user-id";
    
    private readonly ConcurrentDictionary<Type, string> _typeNames = new();
    private readonly IMessageTypeRegistry _messageTypeRegistry;
    private readonly ISerializer _serializer;

    public CustomMessageSerializationStrategy(IMessageTypeRegistry messageTypeRegistry, ISerializer serializer)
    {
        _messageTypeRegistry = messageTypeRegistry;
        _serializer = serializer;
    }

    public SerializedMessage SerializeMessage(IMessage message)
    {
        var messageBody = _serializer.MessageToBytes(message.MessageType, message.GetBody());
        var messageProperties = message.Properties;
        messageProperties.Type = _typeNames.GetOrAdd(message.MessageType, message.MessageType.Name.ToMessageKey());

        return new SerializedMessage(messageProperties, messageBody);
    }

    public IMessage DeserializeMessage(MessageProperties properties, in ReadOnlyMemory<byte> body)
    {
        var type = _messageTypeRegistry.Resolve(properties.Type);
        if (type is null)
        {
            throw new Exception($"Message was not registered for type: '{properties.Type}'.");
        }

        var messageBody = _serializer.BytesToMessage(type, body);
        return MessageFactory.CreateInstance(type, messageBody, properties);
    }
}