using Fast.Shared.Abstractions;
using Fast.Shared.Infrastructure;
using Fast.Shared.Infrastructure.Messaging.RabbitMQ;
using Fast.Shared.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);
var modules = builder.AddModularFramework();

foreach (var module in modules)
{
    module.Register(builder.Services, builder.Configuration);
}

var app = builder.Build().UseModularInfrastructure();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Ping");

foreach (var module in modules)
{
    module.Subscribe(app.Subscribe());
    module.MapInternalApi(app.UseModuleRequests());
    module.MapPublicApi(app);
}

app.Run();