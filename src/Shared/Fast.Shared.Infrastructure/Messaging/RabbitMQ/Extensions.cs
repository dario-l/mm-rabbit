using System.Collections.Generic;
using EasyNetQ;
using EasyNetQ.Consumer;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Infrastructure.Messaging.RabbitMQ.Internals;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using IMessageHandler = Fast.Shared.Infrastructure.Messaging.RabbitMQ.Internals.IMessageHandler;
using MessageHandler = Fast.Shared.Infrastructure.Messaging.RabbitMQ.Internals.MessageHandler;

namespace Fast.Shared.Infrastructure.Messaging.RabbitMQ;

public static class Extensions
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("rabbitmq");
        var options = section.BindOptions<RabbitMQOptions>();
        services.Configure<RabbitMQOptions>(section);
        
        if (!options.Enabled)
        {
            return services;
        }
        
        var messageTypeRegistry = new MessageTypeRegistry();
        
        var bus = RabbitHutch.CreateBus(options.ConnectionString,
            register =>
            {
                register.EnableNewtonsoftJson(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter(new CamelCaseNamingStrategy())
                    },
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                register.Register(typeof(IConventions), typeof(CustomConventions));
                register.Register(typeof(IMessageSerializationStrategy), typeof(CustomMessageSerializationStrategy));
                register.Register(typeof(IHandlerCollectionFactory), typeof(CustomHandlerCollectionFactory));
                register.Register(typeof(IMessageTypeRegistry), messageTypeRegistry);
            });
        
        services.AddSingleton(bus);
        services.AddSingleton<IMessageBroker, RabbitMQMessageBroker>();
        services.AddSingleton<IMessageSubscriber, RabbitMQMessageSubscriber>();
        services.AddSingleton<IMessageHandler, MessageHandler>();
        services.AddSingleton<IMessageTypeRegistry>(messageTypeRegistry);

        return services;
    }
    
    public static IMessageSubscriber Subscribe(this IApplicationBuilder app)
        => app.ApplicationServices.GetRequiredService<IMessageSubscriber>();

    internal static string ToMessageKey(this string messageType) => messageType.Underscore();
}