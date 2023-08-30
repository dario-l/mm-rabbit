using Microsoft.Extensions.DependencyInjection;

namespace Fast.Shared.Infrastructure.Initializers;

public static class Extensions
{
    public static IServiceCollection AddInitializer<T>(this IServiceCollection services) where T : class, IInitializer
        => services.AddTransient<IInitializer, T>();
    
    internal static IServiceCollection AddInitializers(this IServiceCollection services)
        => services.AddHostedService<AppInitializer>();
}