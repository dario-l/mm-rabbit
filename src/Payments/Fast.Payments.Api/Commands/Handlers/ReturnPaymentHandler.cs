using Fast.Payments.Api.Events.Out;
using Fast.Payments.Api.Exceptions;
using Fast.Payments.Api.Services;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Fast.Payments.Api.Commands.Handlers;

internal sealed class ReturnPaymentHandler : ICommandHandler<ReturnPayment>
{
    private readonly Random _random = new();
    private readonly PaymentsProcessor _processor;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<ReturnPaymentHandler> _logger;

    public ReturnPaymentHandler(PaymentsProcessor processor, IMessageBroker messageBroker,
        ILogger<ReturnPaymentHandler> logger)
    {
        _processor = processor;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(ReturnPayment command, CancellationToken cancellationToken = default)
    {
        var rideId = _processor.Get(command.PaymentId);
        if (!rideId.HasValue)
        {
            throw new PaymentNotFoundException(command.PaymentId);
        }
        
        _logger.LogInformation($"Returning the payment with ID: {command.PaymentId}...");
        await Task.Delay(TimeSpan.FromMilliseconds(_random.Next(500, 5000)), cancellationToken);
        _logger.LogInformation($"Returned the payment with ID: {command.PaymentId}.");
        await _messageBroker.SendAsync(new PaymentReturned(command.PaymentId, rideId.Value), cancellationToken);
    }
}