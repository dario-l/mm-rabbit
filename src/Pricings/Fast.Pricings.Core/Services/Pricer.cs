using Fast.Pricings.Core.Clients;
using Fast.Pricings.Core.DTO;
using Fast.Pricings.Core.Events.Out;
using Fast.Pricings.Core.Exceptions;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Time;

namespace Fast.Pricings.Core.Services;

internal sealed class Pricer : IPricer
{
    private readonly IPricingEngine _engine;
    private readonly IDriversClient _driversClient;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;

    public Pricer(IPricingEngine engine, IDriversClient driversClient, IClock clock, IMessageBroker messageBroker)
    {
        _engine = engine;
        _driversClient = driversClient;
        _clock = clock;
        _messageBroker = messageBroker;
    }

    public async Task<decimal> CalculateAsync(CalculatePricing command)
    {
        var drivers = await _driversClient.GetAllAsync();
        if (!drivers.Any(x => x.Available))
        {
            throw new CannotCalculatePricingException();
        }

        var now = _clock.CurrentDate();
        var additionalFee = now.Hour is >= 15 and <= 18 ? 5 : 0;
        var availableDrivers = drivers.Count(x => x.Available);
        var unavailableDrivers = drivers.Length - availableDrivers;
        additionalFee += availableDrivers > unavailableDrivers ? 0 : unavailableDrivers - availableDrivers;
        var pricing = 20 * _engine.Multiplier + additionalFee;
        await _messageBroker.SendAsync(new PricingCalculated(command.PricingId, pricing));
        
        return pricing;
    }
}