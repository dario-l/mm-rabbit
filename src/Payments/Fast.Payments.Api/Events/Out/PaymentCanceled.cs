using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Payments.Api.Events.Out;

[Message("payment_canceled", "payments")]
internal sealed record PaymentCanceled(long PaymentId, long RideId) : IEvent;