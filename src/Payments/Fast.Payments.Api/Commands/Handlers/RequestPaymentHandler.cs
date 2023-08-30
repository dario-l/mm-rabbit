using Fast.Payments.Api.Events.Out;
using Fast.Payments.Api.Services;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Fast.Payments.Api.Commands.Handlers;

internal sealed class RequestPaymentHandler : ICommandHandler<RequestPayment>
{
    private readonly Random _random = new();
    private readonly PaymentsProcessor _processor;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<RequestPaymentHandler> _logger;

    public RequestPaymentHandler(PaymentsProcessor processor, IMessageBroker messageBroker,
        ILogger<RequestPaymentHandler> logger)
    {
        _processor = processor;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(RequestPayment command, CancellationToken cancellationToken = default)
    {
        var paymentId = _processor.Create(command.RideId);
        _logger.LogInformation($"Requesting the payment with ID: {paymentId}, ride ID: {command.RideId}...");
        await Task.Delay(TimeSpan.FromMilliseconds(_random.Next(500, 5000)), cancellationToken);
        _logger.LogInformation($"Requested the payment with ID: {paymentId}, ride ID: {command.RideId}.");
        await _messageBroker.SendAsync(new PaymentRequested(paymentId, command.RideId), cancellationToken);
    }
}