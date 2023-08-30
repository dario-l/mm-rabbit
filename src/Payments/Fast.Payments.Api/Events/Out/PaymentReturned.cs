using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Payments.Api.Events.Out;

[Message("payment_returned", "payments")]
internal sealed record PaymentReturned(long PaymentId, long RideId) : IEvent;