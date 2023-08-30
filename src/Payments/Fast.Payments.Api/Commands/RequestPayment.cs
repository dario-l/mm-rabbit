using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Payments.Api.Commands;

[Message("request_payment", "payments", "payments.request_payment")]
internal sealed record RequestPayment(long RideId, decimal Amount) : ICommand;