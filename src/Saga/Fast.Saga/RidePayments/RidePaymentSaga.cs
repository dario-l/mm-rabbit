using Chronicle;
using Fast.Saga.Messages;
using Fast.Shared.Abstractions.Messaging;
namespace Fast.Saga.RidePayments;

internal sealed class RidePaymentSagaData
{
    public long PaymentId { get; set; }
}

internal sealed class RidePaymentSaga : Saga<RidePaymentSagaData>,
    ISagaStartAction<RideConfirmed>,
    ISagaAction<PaymentRequested>,
    ISagaAction<RideFinished>,
    ISagaAction<PaymentProcessed>
{
    private readonly IMessageBroker _messageBroker;

    public RidePaymentSaga(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }

    public override SagaId ResolveId(object message, ISagaContext context)
        => message switch
        {
            RideConfirmed m => m.RideId.ToString(),
            PaymentRequested m => m.RideId.ToString(),
            RideFinished m => m.RideId.ToString(),
            PaymentProcessed m => m.RideId.ToString(),
            _ => throw new InvalidOperationException("Unsupported message.")
        };

    public Task HandleAsync(RideConfirmed message, ISagaContext context)
        => _messageBroker.SendAsync(new RequestPayment(message.RideId, message.Price));

    public Task CompensateAsync(RideConfirmed message, ISagaContext context)
        => Task.CompletedTask;

    public Task HandleAsync(PaymentRequested message, ISagaContext context)
    {
        Data.PaymentId = message.PaymentId;
        return Task.CompletedTask;
    }

    public Task CompensateAsync(PaymentRequested message, ISagaContext context)
        => Task.CompletedTask;

    public Task HandleAsync(RideFinished message, ISagaContext context)
        => _messageBroker.SendAsync(new ProcessPayment(Data.PaymentId));

    public Task CompensateAsync(RideFinished message, ISagaContext context)
        => Task.CompletedTask;

    public Task HandleAsync(PaymentProcessed message, ISagaContext context)
        => CompleteAsync();

    public Task CompensateAsync(PaymentProcessed message, ISagaContext context)
        => Task.CompletedTask;
}