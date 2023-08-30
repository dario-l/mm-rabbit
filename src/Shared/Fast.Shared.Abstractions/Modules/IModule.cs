using Fast.Shared.Abstractions.Messaging;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Shared.Abstractions.Modules;

public interface IModule
{
    void Register(IServiceCollection services, IConfiguration configuration);
    void MapInternalApi(IModuleEndpointsBuilder endpoints);
    void MapPublicApi(IEndpointRouteBuilder endpoints);
    void Subscribe(IMessageSubscriber subscriber);
}