using Fast.Shared.Abstractions.Exceptions;

namespace Fast.Rides.Application.Exceptions;

internal sealed class RideRequestNotFoundException : CustomException
{
    public long Id { get; }

    public RideRequestNotFoundException(long id) : base($"Ride request: {id} was not found.")
    {
        Id = id;
    }
}