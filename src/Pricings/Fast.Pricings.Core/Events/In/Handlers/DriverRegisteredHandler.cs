using Fast.Pricings.Core.Services;
using Fast.Shared.Abstractions.Events;

namespace Fast.Pricings.Core.Events.In.Handlers;

internal sealed class DriverRegisteredHandler : IEventHandler<DriverRegistered>
{
    private readonly IPricingEngine _pricingEngine;

    public DriverRegisteredHandler(IPricingEngine pricingEngine)
    {
        _pricingEngine = pricingEngine;
    }
    
    public Task HandleAsync(DriverRegistered @event, CancellationToken cancellationToken = default)
    {
        _pricingEngine.IncreaseAvailableDrivers();
        return Task.CompletedTask;
    }
}