using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Saga.Messages;

[Message("payment_processed", "payments", "saga.payments.payment_processed")]
internal sealed record PaymentProcessed(long PaymentId, long RideId) : IEvent;