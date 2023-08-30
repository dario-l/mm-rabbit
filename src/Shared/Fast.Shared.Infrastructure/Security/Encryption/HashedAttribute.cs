using System;

namespace Fast.Shared.Infrastructure.Security.Encryption;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class HashedAttribute : Attribute
{
}