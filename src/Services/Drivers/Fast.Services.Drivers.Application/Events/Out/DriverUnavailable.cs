using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Services.Drivers.Application.Events.Out;

[Message("driver_unavailable", "drivers")]
internal sealed record DriverUnavailable(long DriverId) : IEvent;