using System;

namespace Fast.Shared.Infrastructure.Serialization;

public interface IJsonSerializer
{
    string Serialize<T>(T value);
    T? Deserialize<T>(string value);
    object? Deserialize(string value, Type type);
}