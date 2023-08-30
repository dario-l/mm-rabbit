using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Services.Drivers.Application.Events.Out;

[Message("driver_available", "drivers")]
internal sealed record DriverAvailable(long DriverId) : IEvent;