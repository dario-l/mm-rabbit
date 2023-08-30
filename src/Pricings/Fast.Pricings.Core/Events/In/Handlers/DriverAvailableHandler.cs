using Fast.Pricings.Core.Services;
using Fast.Shared.Abstractions.Events;

namespace Fast.Pricings.Core.Events.In.Handlers;

internal sealed class DriverAvailableHandler : IEventHandler<DriverAvailable>
{
    private readonly IPricingEngine _pricingEngine;

    public DriverAvailableHandler(IPricingEngine pricingEngine)
    {
        _pricingEngine = pricingEngine;
    }
    
    public Task HandleAsync(DriverAvailable @event, CancellationToken cancellationToken = default)
    {
        _pricingEngine.IncreaseAvailableDrivers();
        return Task.CompletedTask;
    }
}