using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Payments.Api.Commands;

[Message("return_payment", "payments", "payments.return_payment")]
internal sealed record ReturnPayment(long PaymentId) : ICommand;