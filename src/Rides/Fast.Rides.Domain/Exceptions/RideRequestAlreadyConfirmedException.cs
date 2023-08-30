using Fast.Shared.Abstractions.Exceptions;

namespace Fast.Rides.Domain.Exceptions;

internal sealed class RideRequestAlreadyConfirmedException : CustomException
{
    public long Id { get; }

    public RideRequestAlreadyConfirmedException(long id) : base($"Ride request: {id} was already confirmed.")
    {
        Id = id;
    }
}