using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Saga.Messages;

[Message("ride_finished", "rides", "saga.rides.ride_finished")]
internal sealed record RideFinished(long RideId, long CustomerId, long DriverId) : IEvent;