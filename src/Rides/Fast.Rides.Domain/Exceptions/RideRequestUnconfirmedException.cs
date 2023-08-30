using Fast.Shared.Abstractions.Exceptions;

namespace Fast.Rides.Domain.Exceptions;

internal sealed class RideRequestUnconfirmedException : CustomException
{
    public long Id { get; }

    public RideRequestUnconfirmedException(long id) : base($"Ride request: {id} was not confirmed.")
    {
        Id = id;
    }
}