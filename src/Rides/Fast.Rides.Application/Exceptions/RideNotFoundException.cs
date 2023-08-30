using Fast.Shared.Abstractions.Exceptions;

namespace Fast.Rides.Application.Exceptions;

internal sealed class RideNotFoundException : CustomException
{
    public long Id { get; }

    public RideNotFoundException(long id) : base($"Ride: {id} was not found.")
    {
        Id = id;
    }
}