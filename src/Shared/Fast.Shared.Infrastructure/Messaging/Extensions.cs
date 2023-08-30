using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Infrastructure.Messaging.Contexts;
using Fast.Shared.Infrastructure.Messaging.Dispatchers;
using Fast.Shared.Infrastructure.Messaging.Subscribers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Shared.Infrastructure.Messaging;

internal static class Extensions
{
    private const string SectionName = "messaging";

    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        services.Configure<MessagingOptions>(section);
        var messagingOptions = section.BindOptions<MessagingOptions>();

        // services.AddTransient<IMessageBroker, InMemoryMessageBroker>();
        services.AddTransient<IAsyncMessageDispatcher, AsyncMessageDispatcher>();
        services.AddSingleton<IMessageChannel, MessageChannel>();
        services.AddSingleton<IMessageContextProvider, MessageContextProvider>();
        services.AddSingleton<IMessageContextRegistry, MessageContextRegistry>();
        services.AddSingleton<IMessageSubscriber, DefaultMessageSubscriber>();

        if (messagingOptions.UseAsyncDispatcher)
        {
            services.AddHostedService<AsyncDispatcherJob>();
        }

        return services;
    }
}