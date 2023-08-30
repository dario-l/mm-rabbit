using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Saga.Messages;

[Message("payment_requested", "payments", "saga.payments.payment_requested")]
internal sealed record PaymentRequested(long PaymentId, long RideId) : IEvent;