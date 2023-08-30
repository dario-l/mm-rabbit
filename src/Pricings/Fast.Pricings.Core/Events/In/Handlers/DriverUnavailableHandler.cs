using Fast.Pricings.Core.Services;
using Fast.Shared.Abstractions.Events;

namespace Fast.Pricings.Core.Events.In.Handlers;

internal sealed class DriverUnavailableHandler : IEventHandler<DriverUnavailable>
{
    private readonly IPricingEngine _pricingEngine;

    public DriverUnavailableHandler(IPricingEngine pricingEngine)
    {
        _pricingEngine = pricingEngine;
    }
    
    public Task HandleAsync(DriverUnavailable @event, CancellationToken cancellationToken = default)
    {
        _pricingEngine.DecreaseAvailableDrivers();
        return Task.CompletedTask;
    }
}