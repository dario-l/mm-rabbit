using Chronicle;
using Fast.Saga.Messages;
using Fast.Shared.Abstractions.Events;

namespace Fast.Saga;

internal sealed class EventsHandler : 
    IEventHandler<RideConfirmed>,
    IEventHandler<RideFinished>,
    IEventHandler<PaymentRequested>,
    IEventHandler<PaymentProcessed>
{
    private readonly ISagaCoordinator _sagaCoordinator;

    public EventsHandler(ISagaCoordinator sagaCoordinator)
    {
        _sagaCoordinator = sagaCoordinator;
    }

    public Task HandleAsync(RideConfirmed @event, CancellationToken cancellationToken = default)
        => _sagaCoordinator.ProcessAsync(@event, SagaContext.Empty);

    public Task HandleAsync(RideFinished @event, CancellationToken cancellationToken = default)
        => _sagaCoordinator.ProcessAsync(@event, SagaContext.Empty);
    
    public Task HandleAsync(PaymentRequested @event, CancellationToken cancellationToken = default)
        => _sagaCoordinator.ProcessAsync(@event, SagaContext.Empty);

    public Task HandleAsync(PaymentProcessed @event, CancellationToken cancellationToken = default)
        => _sagaCoordinator.ProcessAsync(@event, SagaContext.Empty);
}