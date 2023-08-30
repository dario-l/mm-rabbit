using Fast.Shared.Abstractions.Exceptions;

namespace Fast.Rides.Domain.Exceptions;

internal sealed class RideAlreadyFinishedException : CustomException
{
    public long Id { get; }

    public RideAlreadyFinishedException(long id) : base($"Ride: {id} was already finished.")
    {
        Id = id;
    }
}