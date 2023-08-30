using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Pricings.Core.Events.In;

[Message("driver_registered", "drivers", "pricings.drivers.driver_registered")]
internal sealed record DriverRegistered(long DriverId) : IEvent;