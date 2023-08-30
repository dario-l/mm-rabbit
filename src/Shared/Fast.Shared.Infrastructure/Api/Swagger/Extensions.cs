using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Fast.Shared.Infrastructure.Api.Swagger;

public static class Extensions
{
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("swagger");
        var options = section.BindOptions<SwaggerOptions>();
        services.Configure<SwaggerOptions>(section);
        
        if (!options.Enabled)
        {
            return services;
        }
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(swagger =>
        {
            swagger.SchemaFilter<ExcludePropertiesFilter>();
            swagger.EnableAnnotations();
            swagger.CustomSchemaIds(x => x.FullName);
            swagger.SwaggerDoc(options.Version, new OpenApiInfo
            {
                Title = options.Title,
                Version = options.Version
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<SwaggerOptions>>().Value;
        if (!options.Enabled)
        {
            return app;
        }
        
        app.UseSwagger();
        
        var endpoint = $"/swagger/{options.Version}/swagger.json";
        if (options.ReDocEnabled)
        {
            app.UseReDoc(reDoc =>
            {
                reDoc.RoutePrefix = string.IsNullOrWhiteSpace(options.Route) ? "swagger" : options.Route;
                reDoc.SpecUrl(endpoint);
                reDoc.DocumentTitle = options.Title;
            });
        }
        else
        {
            app.UseSwaggerUI(swagger =>
            {
                swagger.RoutePrefix = string.IsNullOrWhiteSpace(options.Route) ? "swagger" : options.Route;
                swagger.SwaggerEndpoint(endpoint, options.Title);
                swagger.DocumentTitle = options.Title;
            });
        }

        return app;
    }
}