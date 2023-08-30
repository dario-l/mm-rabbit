using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Services.Drivers.Application.Events.In;

[Message("ride_finished", "rides", "drivers.rides.ride_finished")]
internal sealed record RideFinished(long RideId, long CustomerId, long DriverId) : IEvent;