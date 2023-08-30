using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Pricings.Core.Events.In;

[Message("driver_unavailable", "drivers", "pricings.drivers.driver_unavailable")]
internal sealed record DriverUnavailable(long DriverId) : IEvent;