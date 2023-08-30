using Fast.Rides.Application.Clients;
using Fast.Rides.Infrastructure.Clients;
using Fast.Rides.Infrastructure.EF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Rides.Infrastructure;

internal static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddSingleton<IDriversClient, DriversClient>();
        services.AddSingleton<IPricingsClient, PricingsClient>();

        return services;
    }
}