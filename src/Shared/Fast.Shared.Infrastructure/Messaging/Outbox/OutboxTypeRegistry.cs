using System;
using System.Collections.Generic;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Shared.Infrastructure.Messaging.Outbox;

internal sealed class OutboxTypeRegistry
{
    private readonly Dictionary<string, Type> _types = new();

    public void Register<T>() where T : IOutbox => _types[GetKey<T>()] = typeof(T);

    public Type? Resolve(IMessage message) => Resolve(GetKey(message.GetType()));
    public Type? Resolve(string module) => _types.TryGetValue(module, out var type) ? type : default;

    private static string GetKey<T>() => GetKey(typeof(T));

    private static string GetKey(Type type)
        => type.IsGenericType
            ? $"{type.GenericTypeArguments[0].GetModuleName()}"
            : $"{type.GetModuleName()}";
}