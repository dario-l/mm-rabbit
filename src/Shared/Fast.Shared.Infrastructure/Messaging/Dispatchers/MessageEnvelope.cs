using Fast.Shared.Abstractions.Messaging;

namespace Fast.Shared.Infrastructure.Messaging.Dispatchers;

internal sealed record MessageEnvelope(IMessage Message, IMessageContext MessageContext);