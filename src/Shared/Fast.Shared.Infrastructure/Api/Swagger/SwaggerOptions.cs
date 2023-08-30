namespace Fast.Shared.Infrastructure.Api.Swagger;

public sealed class SwaggerOptions
{
    public bool Enabled { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public bool ReDocEnabled { get; set; }
}