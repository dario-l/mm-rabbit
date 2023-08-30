using Fast.Rides.Domain.Repositories;
using Fast.Rides.Infrastructure.EF.Repositories;
using Fast.Shared.Infrastructure.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Rides.Infrastructure.EF;

internal static class Extensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgres<RidesDbContext>(configuration);
        services.AddScoped<IRideRepository, RideRepository>();
        services.AddScoped<IRideRequestRepository, RideRequestRepository>();

        return services;
    }
}