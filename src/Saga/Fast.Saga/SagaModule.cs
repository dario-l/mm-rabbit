using Chronicle;
using Fast.Saga.Messages;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Saga;

internal sealed class SagaModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddChronicle();
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
            .Event<PaymentProcessed>()
            .Event<PaymentRequested>()
            .Event<RideConfirmed>()
            .Event<RideFinished>();
    }
}