using System;

namespace Fast.Shared.Abstractions.Exceptions;

public abstract class CustomException : Exception
{
    protected CustomException(string message) : base(message)
    {
    }
}