using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Pricings.Core.Events.In;

[Message("driver_available", "drivers", "pricings.drivers.driver_available")]
internal sealed record DriverAvailable(long DriverId) : IEvent;