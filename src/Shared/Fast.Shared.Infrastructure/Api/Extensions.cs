using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Shared.Infrastructure.Api;

internal static class Extensions
{
    private const string Policy = "cors";
    private const string SectionName = "cors";
    
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        var corsOptions = section.BindOptions<CorsOptions>();
        services.Configure<CorsOptions>(section);
            
        return services
            .AddCors(cors =>
            {
                var allowedHeaders = corsOptions.AllowedHeaders ?? Enumerable.Empty<string>();
                var allowedMethods = corsOptions.AllowedMethods ?? Enumerable.Empty<string>();
                var allowedOrigins = corsOptions.AllowedOrigins ?? Enumerable.Empty<string>();
                var exposedHeaders = corsOptions.ExposedHeaders ?? Enumerable.Empty<string>();
                cors.AddPolicy(Policy, corsBuilder =>
                {
                    var origins = allowedOrigins.ToArray();
                    if (corsOptions.AllowCredentials && origins.FirstOrDefault() != "*")
                    {
                        corsBuilder.AllowCredentials();
                    }
                    else
                    {
                        corsBuilder.DisallowCredentials();
                    }

                    corsBuilder.WithHeaders(allowedHeaders.ToArray())
                        .WithMethods(allowedMethods.ToArray())
                        .WithOrigins(origins.ToArray())
                        .WithExposedHeaders(exposedHeaders.ToArray());
                });
            });
    }

    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app) => app.UseCors(Policy);
    
    public static string GetUserIpAddress(this HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        if (context.Request.Headers.TryGetValue("x-forwarded-for", out var forwardedFor))
        {
            var ipAddresses = forwardedFor.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries);
            if (ipAddresses.Any())
            {
                ipAddress = ipAddresses[0];
            }
        }

        return ipAddress ?? string.Empty;
    }
}