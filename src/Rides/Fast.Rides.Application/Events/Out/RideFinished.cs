using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Rides.Application.Events.Out;

[Message("ride_finished", "rides")]
internal sealed record RideFinished(long RideId, long CustomerId, long DriverId) : IEvent;