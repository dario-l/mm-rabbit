using System;
using System.Linq;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Modules;
using Fast.Shared.Abstractions.Time;
using Fast.Shared.Infrastructure.Contexts;
using Fast.Shared.Infrastructure.Messaging.Contexts;
using Fast.Shared.Infrastructure.Messaging.Dispatchers;
using Fast.Shared.Infrastructure.Serialization;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fast.Shared.Infrastructure.Messaging.Outbox;

internal sealed class EfOutbox<T> : IOutbox where T : DbContext
{
    private readonly T _dbContext;
    private readonly DbSet<OutboxMessage> _set;
    private readonly IMessageContextRegistry _messageContextRegistry;
    private readonly IMessageContextProvider _messageContextProvider;
    private readonly IClock _clock;
    private readonly IModuleClient _moduleClient;
    private readonly IAsyncMessageDispatcher _asyncMessageDispatcher;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ILogger<EfOutbox<T>> _logger;
    private readonly bool _useAsyncDispatcher;

    public bool Enabled { get; }

    public EfOutbox(T dbContext, IMessageContextRegistry messageContextRegistry,
        IMessageContextProvider messageContextProvider, IClock clock, IModuleClient moduleClient,
        IAsyncMessageDispatcher asyncMessageDispatcher, IJsonSerializer jsonSerializer,
        IOptions<MessagingOptions> messagingOptions, IOptions<OutboxOptions> outboxOptions,  ILogger<EfOutbox<T>> logger)
    {
        _dbContext = dbContext;
        _set = dbContext.Set<OutboxMessage>();
        _messageContextRegistry = messageContextRegistry;
        _messageContextProvider = messageContextProvider;
        _clock = clock;
        _moduleClient = moduleClient;
        _asyncMessageDispatcher = asyncMessageDispatcher;
        _jsonSerializer = jsonSerializer;
        _useAsyncDispatcher = messagingOptions.Value.UseAsyncDispatcher;
        _logger = logger;
        Enabled = outboxOptions.Value.Enabled;
    }

    public async Task SaveAsync(params IMessage[]? messages)
    {
        var module = _dbContext.GetModuleName();
        if (!Enabled)
        {
            _logger.LogWarning($"Outbox is disabled ('{module}'), outgoing messages won't be saved.");
            return;
        }

        if (messages is null || !messages.Any())
        {
            _logger.LogWarning($"No messages have been provided to be saved to the outbox ('{module}').");
            return;
        }

        var outboxMessages = messages.Select(x =>
            {
                var context = _messageContextProvider.Get(x);
                if (context is null)
                {
                    throw new InvalidOperationException("Message context is null.");
                }
                
                return new OutboxMessage
                {
                    Id = context.MessageId,
                    ActivityId = context.Context.ActivityId,
                    Name = x.GetType().Name.Underscore(),
                    Data = _jsonSerializer.Serialize((object)x),
                    Type = x.GetType().AssemblyQualifiedName!,
                    CreatedAt = _clock.CurrentDate(),
                    TraceId = context.Context.TraceId ?? string.Empty,
                    UserId = context.Context.UserId ?? string.Empty
                };
            }).ToArray();

        if (!outboxMessages.Any())
        {
            _logger.LogWarning($"No messages have been provided to be saved to the outbox ('{module}').");
            return;
        }

        await _set.AddRangeAsync(outboxMessages);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Saved {outboxMessages.Length} messages to the outbox ('{module}').");
    }

    public async Task PublishUnsentAsync()
    {
        var module = _dbContext.GetModuleName();
        if (!Enabled)
        {
            _logger.LogWarning($"Outbox is disabled ('{module}'), outgoing messages won't be sent.");
            return;
        }
            
        var unsentMessages = await _set.Where(x => x.SentAt == null).ToListAsync();
        if (!unsentMessages.Any())
        {
            _logger.LogTrace($"No unsent messages found in outbox ('{module}').");
            return;
        }

        _logger.LogTrace($"Found {unsentMessages.Count} unsent messages in outbox ('{module}'), sending...");
        foreach (var outboxMessage in unsentMessages)
        {
            var type = Type.GetType(outboxMessage.Type);
            var message = _jsonSerializer.Deserialize(outboxMessage.Data, type!) as IMessage;
            if (message is null)
            {
                _logger.LogError($"Invalid message type in outbox ('{module}'): '{type!.Name}', name: '{outboxMessage.Name}', " +
                                 $"ID: '{outboxMessage.Id}' ('{module}').");
                continue;
            }

            var messageId = outboxMessage.Id;
            var activityId = outboxMessage.ActivityId;
            var sentAt = _clock.CurrentDate();
            var name = message.GetType().Name.Underscore();
            var context = new Context(activityId, outboxMessage.TraceId, outboxMessage.UserId);
            _messageContextRegistry.Set(message, new MessageContext(messageId, context));
                
            _logger.LogInformation("Sending a message from outbox ('{Module}'): {Name} [Message ID: {MessageId}, Activity ID: {ActivityID}]...",
                module, name, messageId, activityId);

            if (_useAsyncDispatcher)
            {
                await _asyncMessageDispatcher.PublishAsync(message);
            }
            else
            {
                await _moduleClient.PublishAsync(message);
            }
                
            outboxMessage.SentAt = sentAt;
            _set.Update(outboxMessage);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task CleanupAsync(DateTime? to = null)
    {
        var module = _dbContext.GetModuleName();
        if (!Enabled)
        {
            _logger.LogWarning($"Outbox is disabled ('{module}'), outgoing messages won't be cleaned up.");
            return;
        }

        var dateTo = to ?? _clock.CurrentDate();
        var sentMessages = await _set.Where(x => x.SentAt != null && x.CreatedAt <= dateTo).ToListAsync();
        if (!sentMessages.Any())
        {
            _logger.LogTrace($"No sent messages found in outbox ('{module}') till: {dateTo}.");
            return;
        }

        _logger.LogTrace($"Found {sentMessages.Count} sent messages in outbox ('{module}') till: {dateTo}, cleaning up...");
        _set.RemoveRange(sentMessages);
        await _dbContext.SaveChangesAsync();
        _logger.LogTrace($"Removed {sentMessages.Count} sent messages from outbox ('{module}') till: {dateTo}.");
    }
}