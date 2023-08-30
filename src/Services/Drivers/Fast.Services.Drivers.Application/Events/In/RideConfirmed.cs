using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Services.Drivers.Application.Events.In;

[Message("ride_confirmed", "rides", "drivers.rides.ride_confirmed")]
internal sealed record RideConfirmed(long RideId, long CustomerId, long DriverId) : IEvent;