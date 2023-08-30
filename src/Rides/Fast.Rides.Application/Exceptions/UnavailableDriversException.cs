using Fast.Shared.Abstractions.Exceptions;

namespace Fast.Rides.Application.Exceptions;

internal sealed class UnavailableDriversException : CustomException
{
    public UnavailableDriversException() : base("Drivers are unavailable.")
    {
    }
}