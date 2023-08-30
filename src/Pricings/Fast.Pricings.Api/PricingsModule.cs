using Fast.Pricings.Core;
using Fast.Pricings.Core.DTO;
using Fast.Pricings.Core.Events.In;
using Fast.Pricings.Core.Services;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Pricings.Api;

internal sealed class PricingsModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCore();
    }
    
    public void MapInternalApi(IModuleEndpointsBuilder endpoints)
    {
        endpoints.Map<CalculatePricing, PricingDto>(async (command, serviceProvider, _) =>
        {
            var pricer = serviceProvider.GetRequiredService<IPricer>();
            var pricing = await pricer.CalculateAsync(command);
            return new PricingDto(command.PricingId, pricing);
        });
    }
    
    public void MapPublicApi(IEndpointRouteBuilder endpoints)
    {
    }

    public void Subscribe(IMessageSubscriber subscriber)
    {
        subscriber
            .Event<DriverAvailable>()
            .Event<DriverRegistered>()
            .Event<DriverUnavailable>();
    }
}