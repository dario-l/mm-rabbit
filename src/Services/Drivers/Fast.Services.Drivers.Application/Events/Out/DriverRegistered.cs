using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Services.Drivers.Application.Events.Out;

[Message("driver_registered", "drivers")]
internal sealed record DriverRegistered(long DriverId) : IEvent;