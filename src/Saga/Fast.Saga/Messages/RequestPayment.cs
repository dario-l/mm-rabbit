using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Saga.Messages;

[Message("request_payment", "payments")]
internal sealed record RequestPayment(long RideId, decimal Amount) : ICommand;