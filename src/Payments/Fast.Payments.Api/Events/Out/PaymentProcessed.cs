using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Payments.Api.Events.Out;

[Message("payment_processed", "payments")]
internal sealed record PaymentProcessed(long PaymentId, long RideId) : IEvent;