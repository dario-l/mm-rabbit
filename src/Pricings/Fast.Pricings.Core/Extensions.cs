using Fast.Pricings.Core.Clients;
using Fast.Pricings.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Pricings.Core;

internal static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddSingleton<IPricer, Pricer>();
        services.AddSingleton<IPricingEngine, PricingEngine>();
        services.AddSingleton<IDriversClient, DriversClient>();
        
        return services;
    }
}