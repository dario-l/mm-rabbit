using Fast.Payments.Api.Events.Out;
using Fast.Payments.Api.Exceptions;
using Fast.Payments.Api.Services;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Fast.Payments.Api.Commands.Handlers;

internal sealed class ProcessPaymentHandler : ICommandHandler<ProcessPayment>
{
    private readonly Random _random = new();
    private readonly PaymentsProcessor _processor;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<ProcessPaymentHandler> _logger;

    public ProcessPaymentHandler(PaymentsProcessor processor, IMessageBroker messageBroker,
        ILogger<ProcessPaymentHandler> logger)
    {
        _processor = processor;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(ProcessPayment command, CancellationToken cancellationToken = default)
    {
        var rideId = _processor.Get(command.PaymentId);
        if (!rideId.HasValue)
        {
            throw new PaymentNotFoundException(command.PaymentId);
        }
        
        _logger.LogInformation($"Processing the payment with ID: {command.PaymentId}...");
        await Task.Delay(TimeSpan.FromMilliseconds(_random.Next(500, 5000)), cancellationToken);
        _logger.LogInformation($"Processed the payment with ID: {command.PaymentId}.");
        await _messageBroker.SendAsync(new PaymentProcessed(command.PaymentId, rideId.Value), cancellationToken);
    }
}