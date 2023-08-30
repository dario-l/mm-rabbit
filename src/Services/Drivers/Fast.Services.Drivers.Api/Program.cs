using Fast.Services.Drivers.Application.Commands;
using Fast.Services.Drivers.Application.DTO;
using Fast.Services.Drivers.Application.Events.In;
using Fast.Services.Drivers.Application.Queries;
using Fast.Services.Drivers.DataAccess;
using Fast.Shared.Abstractions.Dispatchers;
using Fast.Shared.Abstractions.Identity;
using Fast.Shared.Infrastructure;
using Fast.Shared.Infrastructure.Messaging.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.AddFramework();
builder.Services.AddDataAccess(builder.Configuration);

var app = builder.Build();

app.UseInfrastructure();

var api = app.MapGroup("drivers").WithTags("Drivers");

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

app.Subscribe()
    .Event<RideConfirmed>()
    .Event<RideFinished>();

app.Run();
