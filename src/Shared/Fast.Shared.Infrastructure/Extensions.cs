using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fast.Shared.Abstractions;
using Fast.Shared.Abstractions.Dispatchers;
using Fast.Shared.Abstractions.Identity;
using Fast.Shared.Abstractions.Modules;
using Fast.Shared.Abstractions.Storage;
using Fast.Shared.Abstractions.Time;
using Fast.Shared.Infrastructure.Api;
using Fast.Shared.Infrastructure.Api.Swagger;
using Fast.Shared.Infrastructure.Auth;
using Fast.Shared.Infrastructure.Contexts;
using Fast.Shared.Infrastructure.Dispatchers;
using Fast.Shared.Infrastructure.Exceptions;
using Fast.Shared.Infrastructure.Identity;
using Fast.Shared.Infrastructure.Initializers;
using Fast.Shared.Infrastructure.Kernel;
using Fast.Shared.Infrastructure.Logging;
using Fast.Shared.Infrastructure.Messaging;
using Fast.Shared.Infrastructure.Messaging.Outbox;
using Fast.Shared.Infrastructure.Messaging.RabbitMQ;
using Fast.Shared.Infrastructure.Modules;
using Fast.Shared.Infrastructure.Postgres;
using Fast.Shared.Infrastructure.Security;
using Fast.Shared.Infrastructure.Serialization;
using Fast.Shared.Infrastructure.Storage;
using Fast.Shared.Infrastructure.Time;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fast.Shared.Infrastructure;

public static class Extensions
{
    private const string AppOptionsSection = "app";
        
    public static IModule[] AddModularFramework(this WebApplicationBuilder builder,
        string appOptionsSection = AppOptionsSection)
    {
        builder.Host.ConfigureModules();
        
        var services = builder.Services;
        var configuration = builder.Configuration;
        var assemblies = ModuleLoader.LoadAssemblies(configuration);
        var modules = ModuleLoader.LoadModules(assemblies);
        var disabledModules = configuration.LoadModulesFromSettings().Where(x => !x.Enabled).Select(x => x.Name);
        
        services
            .AddModuleRequests(assemblies)
            .AddControllers()
            .ConfigureApplicationPartManager(manager =>
            {
                var removedParts = new List<ApplicationPart>();
                foreach (var disabledModule in disabledModules)
                {
                    var parts = manager.ApplicationParts.Where(x => x.Name.Contains(disabledModule,
                        StringComparison.InvariantCultureIgnoreCase));
                    removedParts.AddRange(parts);
                }

                foreach (var part in removedParts)
                {
                    manager.ApplicationParts.Remove(part);
                }
                    
                manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
            });

        builder.AddFramework(appOptionsSection, assemblies);
            
        return modules;
    }
    
    public static IServiceCollection AddFramework(this WebApplicationBuilder builder,
        string appOptionsSection = AppOptionsSection, Assembly[]? assemblies = null)
    {
        assemblies ??= AppDomain.CurrentDomain.GetAssemblies().ToArray();
        builder.Host.UseLogging();
        
        var services = builder.Services;
        var configuration = builder.Configuration;

        var section = configuration.GetSection(appOptionsSection);
        services.Configure<AppOptions>(section);
        var appOptions = section.BindOptions<AppOptions>();
        
        var appInfo = new AppInfo(appOptions.Name, appOptions.Version);
        services.AddSingleton(appInfo);
        
        services.AddCorsPolicy(configuration);
        services.AddSwaggerDocs(configuration);
        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddSingleton<IRequestStorage, RequestStorage>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
        services.AddSingleton<IIdGen>(new IdentityGenerator());
        services.AddAuth(configuration);
        services.AddErrorHandling();
        services.AddContext();
        services.AddHandlers(assemblies);
        services.AddDispatchers();
        services.AddDomainEvents(assemblies);
        services.AddMessaging(configuration);
        services.AddSecurity(configuration);
        services.AddOutbox(configuration);
        services.AddPostgres(configuration);
        services.AddRabbitMQ(configuration);
        services.AddDataInitializers();
        services.AddInitializers();
        services.AddSingleton<IClock, UtcClock>();
        services.AddSingleton<IDispatcher, InMemoryDispatcher>();
        services.AddHostedService<AppInitializer>();
        services.AddTransactionalDecorators();
        services.AddLoggingDecorators();

        return services;
    }

    public static WebApplication UseModularInfrastructure(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<IModule>>();
        var modules = app.Configuration.LoadModulesFromSettings().Where(x => x.Enabled).Select(x => x.Name);
        logger.LogInformation("Modules: {Modules}", string.Join(", ", modules));

        return app.UseInfrastructure();
    }
    
    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions()
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        app.UseCorsPolicy();
        app.UseErrorHandling();
        app.UseSwaggerDocs();
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();

        return app;
    }

    private static IEnumerable<(string Name, bool Enabled)> LoadModulesFromSettings(this IConfiguration configuration)
    {
        foreach (var (key, value) in configuration.AsEnumerable())
        {
            if (!key.Contains(":module:enabled") || value is null)
            {
                continue;
            }

            yield return (key.Split(":")[0], bool.Parse(value));
        }
    }

    internal static AppOptions BindAppOptions(this IConfiguration configuration, string sectionName = AppOptionsSection)
        => configuration.BindOptions<AppOptions>(sectionName);

    public static T BindOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
        => BindOptions<T>(configuration.GetSection(sectionName));

    public static T BindOptions<T>(this IConfigurationSection section) where T : new()
    {
        var options = new T();
        section.Bind(options);
        return options;
    }

    public static string GetModuleName(this object? value)
        => value?.GetType().GetModuleName() ?? string.Empty;

    public static string GetModuleName(this Type? type, int splitIndex = 1) 
        => type?.Namespace is null ? string.Empty : type.Namespace.Split(".")[splitIndex].ToLowerInvariant();
}