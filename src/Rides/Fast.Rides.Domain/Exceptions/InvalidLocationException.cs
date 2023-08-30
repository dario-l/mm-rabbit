using Fast.Shared.Abstractions.Exceptions;

namespace Fast.Rides.Domain.Exceptions;

internal sealed class InvalidLocationException : CustomException
{
    public InvalidLocationException() : base("Location is invalid.")
    {
    }
}