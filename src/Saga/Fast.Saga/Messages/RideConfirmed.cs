using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Saga.Messages;

[Message("ride_confirmed", "rides", "saga.rides.ride_confirmed")]
internal sealed record RideConfirmed(long RideId, long CustomerId, long DriverId, decimal Price) : IEvent;