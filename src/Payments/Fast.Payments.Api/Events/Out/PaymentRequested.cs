using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Payments.Api.Events.Out;

[Message("payment_requested", "payments")]
internal sealed record PaymentRequested(long PaymentId, long RideId) : IEvent;