var builder = WebApplication
    .CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetRequiredSection("reverseProxy"));

var app = builder.Build();

app.MapGet("/", () => "Gateway").WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.MapReverseProxy();

app.Run();