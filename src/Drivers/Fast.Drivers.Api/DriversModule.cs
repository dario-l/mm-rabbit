using Fast.Drivers.Application.Commands;
using Fast.Drivers.Application.DTO;
using Fast.Drivers.Application.Events.In;
using Fast.Drivers.Application.Queries;
using Fast.Drivers.DataAccess;
using Fast.Shared.Abstractions.Dispatchers;
using Fast.Shared.Abstractions.Identity;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Drivers.Api;

internal sealed class DriversModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataAccess(configuration);
    }
    
    public void MapInternalApi(IModuleEndpointsBuilder endpoints)
    {
        endpoints.Map<GetDrivers, IEnumerable<DriverDto>>(async (query, serviceProvider, cancellationToken) =>
        {
            var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
            return await dispatcher.QueryAsync(query, cancellationToken);
        });
    }

    public void MapPublicApi(IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("drivers").WithTags("Drivers");

        api.MapGet("", async (IDispatcher dispatcher) => Results.Ok(await dispatcher.QueryAsync(new GetDrivers())))
            .WithName("Get drivers")
            .Produces<DriverDto[]>();

        api.MapGet("{driverId:long}", async (long driverId, IDispatcher dispatcher) =>
            {
                var driver = await dispatcher.QueryAsync(new GetDriver(driverId));
                return driver is null ? Results.NotFound() : Results.Ok(driver);
            })
            .WithName("Get driver")
            .Produces<DriverDto>();

        api.MapPost("", async (RegisterDriver command, IDispatcher dispatcher, IIdGen idGen) =>
            {
                command = command with {DriverId = idGen.Create()};
                await dispatcher.SendAsync(command);
                return Results.CreatedAtRoute("Get driver", new {command.DriverId});
            })
            .WithName("Register driver")
            .Produces(StatusCodes.Status201Created);
    }

    public void Subscribe(IMessageSubscriber subscriber)
    {
        subscriber
            .Event<RideConfirmed>()
            .Event<RideFinished>();
    }
}