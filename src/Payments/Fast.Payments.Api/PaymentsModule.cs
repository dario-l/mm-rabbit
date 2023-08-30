using Fast.Payments.Api.Commands;
using Fast.Payments.Api.Services;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Payments.Api;

internal sealed class PaymentsModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<PaymentsProcessor>();
    }

    public void MapInternalApi(IModuleEndpointsBuilder endpoints)
    {
    }

    public void MapPublicApi(IEndpointRouteBuilder endpoints)
    {
    }

    public void Subscribe(IMessageSubscriber subscriber)
    {
        subscriber
            .Command<CancelPayment>()
            .Command<ProcessPayment>()
            .Command<RequestPayment>()
            .Command<ReturnPayment>();
    }
}