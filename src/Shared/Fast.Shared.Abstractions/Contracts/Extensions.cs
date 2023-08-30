using Microsoft.Extensions.DependencyInjection;

namespace Fast.Shared.Abstractions.Contracts;

public static class Extensions
{
    public static IServiceCollection AddContractValidator<T>(this IServiceCollection services)
        where T : class, IContractValidator => services.AddTransient<IContractValidator, T>();
}