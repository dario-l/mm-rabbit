using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Pricings.Core.Events.Out;

[Message("pricing_calculated", "pricings")]
internal sealed record PricingCalculated(long PricingId, decimal Pricing) : IEvent;