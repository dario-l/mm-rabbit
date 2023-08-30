using System.Reflection;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Dispatchers;
using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Queries;
using Fast.Shared.Abstractions.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Shared.Infrastructure.Dispatchers;

internal static class Extensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly[] assemblies)
    {
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>))
                .WithoutAttribute<DecoratorAttribute>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>))
                .WithoutAttribute<DecoratorAttribute>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>))
                .WithoutAttribute<DecoratorAttribute>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddDispatchers(this IServiceCollection services)
        => services
            .AddSingleton<IDispatcher, InMemoryDispatcher>()
            .AddSingleton<ICommandDispatcher, InMemoryCommandDispatcher>()
            .AddSingleton<IEventDispatcher, InMemoryEventDispatcher>()
            .AddSingleton<IQueryDispatcher, InMemoryQueryDispatcher>();
}