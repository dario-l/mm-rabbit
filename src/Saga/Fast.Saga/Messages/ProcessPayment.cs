using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Saga.Messages;

[Message("process_payment", "payments")]
internal sealed record ProcessPayment(long PaymentId) : ICommand;