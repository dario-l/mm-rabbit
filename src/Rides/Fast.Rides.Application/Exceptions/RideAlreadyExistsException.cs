using Fast.Shared.Abstractions.Exceptions;

namespace Fast.Rides.Application.Exceptions;

internal sealed class RideAlreadyExistsException : CustomException
{
    public long RequestId { get; }

    public RideAlreadyExistsException(long requestId) : base($"Ride for request: {requestId} already exists.")
    {
        RequestId = requestId;
    }
}