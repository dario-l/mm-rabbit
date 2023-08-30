using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Contracts;
using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Modules;
using Fast.Shared.Infrastructure.Contracts;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fast.Shared.Infrastructure.Modules;

public static class Extensions
{
    public static IHostBuilder ConfigureModules(this IHostBuilder builder)
        => builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            foreach (var settings in GetSettings("*"))
            {
                cfg.AddJsonFile(settings);
            }

            foreach (var settings in GetSettings($"*.{ctx.HostingEnvironment.EnvironmentName}"))
            {
                cfg.AddJsonFile(settings);
            }

            IEnumerable<string> GetSettings(string pattern)
                => Directory.EnumerateFiles(ctx.HostingEnvironment.ContentRootPath,
                    $"module.{pattern}.json", SearchOption.AllDirectories);
        });
        
    public static IServiceCollection AddModuleRequests(this IServiceCollection services,
        IEnumerable<Assembly> assemblies)
    {
        services.AddModuleRegistry(assemblies);
        services.AddSingleton<IModuleClient, ModuleClient>();
        services.AddSingleton<IModuleSerializer, JsonModuleSerializer>();
        // services.AddSingleton<IModuleSerializer, MessagePackModuleSerializer>();
        services.AddSingleton<IModuleEndpointsBuilder, ModuleEndpointsBuilder>();

        return services;
    }
    
    public static IModuleEndpointsBuilder UseModuleRequests(this IApplicationBuilder app)
        => app.ApplicationServices.GetRequiredService<IModuleEndpointsBuilder>();

    internal static string GetTopic<T>(this T message) where T : class
    {
        var type = message as Type ?? message.GetType();
        var messageAttribute = type.GetCustomAttribute<MessageAttribute>();

        return string.IsNullOrWhiteSpace(messageAttribute?.Topic) ? type.Name.Underscore() : messageAttribute.Topic;
    }

    private static void AddModuleRegistry(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var registry = new ModuleRegistry();
        var types = assemblies.SelectMany(x => x.GetTypes()).ToArray();
        
        services.AddSingleton<IContractRegistry>(sp => new ContractRegistry(registry,
            types, sp.GetRequiredService<ILogger<ContractRegistry>>()));
            
        var commandTypes = types
            .Where(t => t.IsClass && typeof(ICommand).IsAssignableFrom(t))
            .ToArray();
            
        var eventTypes = types
            .Where(x => x.IsClass && typeof(IEvent).IsAssignableFrom(x))
            .ToArray();

        services.AddSingleton<IModuleRegistry>(sp =>
        {
            var commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
            var commandDispatcherType = commandDispatcher.GetType();
                
            var eventDispatcher = sp.GetRequiredService<IEventDispatcher>();
            var eventDispatcherType = eventDispatcher.GetType();

            foreach (var type in commandTypes)
            {
                registry.AddBroadcastAction(type, (@event, cancellationToken) =>
#pragma warning disable CS8603
#pragma warning disable CS8600
                    (Task) commandDispatcherType.GetMethod(nameof(commandDispatcher.SendAsync))
                        ?.MakeGenericMethod(type)
                        .Invoke(commandDispatcher, new[] {@event, cancellationToken}));
#pragma warning restore CS8600
#pragma warning restore CS8603
            }
                
            foreach (var type in eventTypes)
            {
                registry.AddBroadcastAction(type, (@event, cancellationToken) =>
#pragma warning disable CS8603
#pragma warning disable CS8600
                    (Task) eventDispatcherType.GetMethod(nameof(eventDispatcher.PublishAsync))
                        ?.MakeGenericMethod(type)
                        .Invoke(eventDispatcher, new[] {@event, cancellationToken}));
#pragma warning restore CS8600
#pragma warning restore CS8603
            }

            return registry;
        });
    }
}