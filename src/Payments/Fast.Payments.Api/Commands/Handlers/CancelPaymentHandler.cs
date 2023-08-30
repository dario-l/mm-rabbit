using Fast.Payments.Api.Events.Out;
using Fast.Payments.Api.Exceptions;
using Fast.Payments.Api.Services;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Fast.Payments.Api.Commands.Handlers;

internal sealed class CancelPaymentHandler : ICommandHandler<CancelPayment>
{
    private readonly Random _random = new();
    private readonly PaymentsProcessor _processor;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<CancelPaymentHandler> _logger;

    public CancelPaymentHandler(PaymentsProcessor processor, IMessageBroker messageBroker,
        ILogger<CancelPaymentHandler> logger)
    {
        _processor = processor;
        _messageBroker = messageBroker;
        _logger = logger;
    }
    
    public async Task HandleAsync(CancelPayment command, CancellationToken cancellationToken = default)
    {
        var rideId = _processor.Get(command.PaymentId);
        if (!rideId.HasValue)
        {
            throw new PaymentNotFoundException(command.PaymentId);
        }
        
        _logger.LogInformation($"Cancelling the payment with ID: {command.PaymentId}...");
        await Task.Delay(TimeSpan.FromMilliseconds(_random.Next(500, 5000)), cancellationToken);
        _logger.LogInformation($"Cancelled the payment with ID: {command.PaymentId}.");
        await _messageBroker.SendAsync(new PaymentCanceled(command.PaymentId, rideId.Value), cancellationToken);
    }
}