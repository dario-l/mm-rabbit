using System;

namespace Fast.Shared.Abstractions.Types;

// Marker attribute - avoid resolving multiple handlers with additional decorators
[AttributeUsage(AttributeTargets.Class)]
public sealed class DecoratorAttribute : Attribute
{
}