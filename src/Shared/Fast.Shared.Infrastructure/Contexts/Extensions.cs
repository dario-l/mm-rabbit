using Fast.Shared.Abstractions.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Shared.Infrastructure.Contexts;

internal static class Extensions
{
    public static IServiceCollection AddContext(this IServiceCollection services)
    {
        services.AddSingleton<IContextProvider, ContextProvider>();
        services.AddSingleton<ContextAccessor>();
        
        return services;
    }
}