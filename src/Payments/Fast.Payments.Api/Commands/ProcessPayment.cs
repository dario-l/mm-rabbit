using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Payments.Api.Commands;

[Message("process_payment", "payments", "payments.process_payment")]
internal sealed record ProcessPayment(long PaymentId) : ICommand;