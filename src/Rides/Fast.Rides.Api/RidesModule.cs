using Fast.Rides.Application.Commands;
using Fast.Rides.Application.DTO;
using Fast.Rides.Application.Queries;
using Fast.Rides.Infrastructure;
using Fast.Shared.Abstractions.Dispatchers;
using Fast.Shared.Abstractions.Identity;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Rides.Api;

internal sealed class RidesModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
    }

    public void MapInternalApi(IModuleEndpointsBuilder endpoints)
    {
    }

    public void MapPublicApi(IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("rides").WithTags("Rides");
        
        api.MapGet("{rideId:long}", async (long rideId, IDispatcher dispatcher) =>
            {
                var ride = await dispatcher.QueryAsync(new GetRide(rideId));
                return ride is null ? Results.NotFound() : Results.Ok(ride);
            })
            .WithName("Get ride")
            .Produces<RideDto>();
        
        api.MapGet("requests/{rideRequestId:long}", async (long rideRequestId, IDispatcher dispatcher) =>
            {
                var rideRequest = await dispatcher.QueryAsync(new GetRideRequest(rideRequestId));
                return rideRequest is null ? Results.NotFound() : Results.Ok(rideRequest);
            })
            .WithName("Get ride request")
            .Produces<RideRequestDto>();

        api.MapPost("requests", async (RequestRide command, IDispatcher dispatcher, IIdGen idGen) =>
            {
                command = command with {RideRequestId = idGen.Create()};
                await dispatcher.SendAsync(command);
                return Results.CreatedAtRoute("Get ride request", new {command.RideRequestId});
            })
            .WithName("Request ride")
            .Produces(StatusCodes.Status201Created);

        api.MapPut("requests/{rideRequestId:long}/confirm", async (long rideRequestId, IDispatcher dispatcher) =>
            {
                await dispatcher.SendAsync(new ConfirmRideByCustomer(rideRequestId));
                return Results.NoContent();
            })
            .WithName("Confirm ride by customer")
            .Produces(StatusCodes.Status204NoContent);
        
        api.MapPost("", async (ConfirmRideByDriver command, IDispatcher dispatcher, IIdGen idGen) =>
            {
                command = command with {RideId = idGen.Create()};
                await dispatcher.SendAsync(command);
                return Results.CreatedAtRoute("Get ride", new {command.RideId});
            })
            .WithName("Confirm ride by driver")
            .Produces(StatusCodes.Status201Created);
        
        api.MapPut("{rideId:long}/finish", async (long rideId, IDispatcher dispatcher) =>
            {
                await dispatcher.SendAsync(new FinishRide(rideId));
                return Results.NoContent();
            })
            .WithName("Finish ride")
            .Produces(StatusCodes.Status204NoContent);
    }

    public void Subscribe(IMessageSubscriber subscriber)
    {
    }
}