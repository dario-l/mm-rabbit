using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Rides.Application.Events.Out;

[Message("ride_confirmed", "rides")]
internal sealed record RideConfirmed(long RideId, long CustomerId, long DriverId, decimal Price) : IEvent;