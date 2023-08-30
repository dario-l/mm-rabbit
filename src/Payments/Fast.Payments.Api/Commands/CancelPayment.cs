using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Payments.Api.Commands;

[Message("cancel_payment", "payments", "payments.cancel_payment")]
internal sealed record CancelPayment(long PaymentId) : ICommand;