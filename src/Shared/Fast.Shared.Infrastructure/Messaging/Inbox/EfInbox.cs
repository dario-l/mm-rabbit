using System;
using System.Linq;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Time;
using Fast.Shared.Infrastructure.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fast.Shared.Infrastructure.Messaging.Inbox;

internal sealed class EfInbox<T> : IInbox where T : DbContext
{
    private readonly T _dbContext;
    private readonly DbSet<InboxMessage> _set;
    private readonly IClock _clock;
    private readonly ILogger<EfInbox<T>> _logger;

    public bool Enabled { get; }

    public EfInbox(T dbContext, IClock clock, IOptions<OutboxOptions> outboxOptions, ILogger<EfInbox<T>> logger)
    {
        _dbContext = dbContext;
        _set = dbContext.Set<InboxMessage>();
        _clock = clock;
        _logger = logger;
        Enabled = outboxOptions.Value.Enabled;
    }

    public async Task HandleAsync(Guid messageId, string name, Func<Task> handler)
    {
        var module = _dbContext.GetModuleName();
        if (!Enabled)
        {
            _logger.LogWarning($"Inbox is disabled ('{module}'), incoming messages won't be processed.");
            return;
        }
        
        _logger.LogTrace($"Received a message with ID: '{messageId}' to be processed ('{module}').");
        if (await _set.AnyAsync(m => m.Id == messageId && m.ProcessedAt != null))
        {
            _logger.LogTrace($"Message with ID: '{messageId}' was already processed ('{module}').");
            return;
        }
        
        // Transaction is handled by UnitOfWork and TransactionalDecorator
        
        var inboxMessage = new InboxMessage
        {
            Id = messageId,
            Name = name,
            ReceivedAt = _clock.CurrentDate()
        };

        _logger.LogTrace($"Processing a message with ID: '{messageId}' ('{module}')...");
        await _set.AddAsync(inboxMessage);
        await _dbContext.SaveChangesAsync();

        try
        {
            await handler();
            inboxMessage.ProcessedAt = _clock.CurrentDate();
            _set.Update(inboxMessage);
            await _dbContext.SaveChangesAsync();
            _logger.LogTrace($"Processed a message with ID: '{messageId}' ('{module}').");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"There was an error when processing a message with ID: '{messageId}' ('{module}').");
            throw;
        }
    }

    public async Task CleanupAsync(DateTime? to = null)
    {
        var module = _dbContext.GetModuleName();
        if (!Enabled)
        {
            _logger.LogWarning($"Inbox is disabled ('{module}'), incoming messages won't be cleaned up.");
            return;
        }

        var dateTo = to ?? _clock.CurrentDate();
        var sentMessages = await _set.Where(x => x.ReceivedAt <= dateTo).ToListAsync();
        if (!sentMessages.Any())
        {
            _logger.LogTrace($"No received messages found in inbox ('{module}') till: {dateTo}.");
            return;
        }

        _logger.LogInformation($"Found {sentMessages.Count} received messages in inbox ('{module}') till: {dateTo}, cleaning up...");
        _set.RemoveRange(sentMessages);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Removed {sentMessages.Count} received messages from inbox ('{module}') till: {dateTo}.");
    }
}