using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fast.Shared.Abstractions.Types;
using Humanizer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fast.Shared.Infrastructure.Api.Swagger;

internal sealed class ExcludePropertiesFilter : ISchemaFilter
{
    private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;
    private static readonly ConcurrentDictionary<Type, IDictionary<string, OpenApiSchema>> Properties = new();

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties.Count == 0)
        {
            return;
        }

        if (Properties.TryGetValue(context.Type, out var properties))
        {
            schema.Properties = properties;
            return;
        }

        var excludedProperties = context.Type
            .GetProperties(Flags)
            .Where(x => x.GetCustomAttribute<HiddenAttribute>() is not null)
            .Select(x => x.Name.Camelize());

        foreach (var excludedProperty in excludedProperties)
        {
            if (schema.Properties.ContainsKey(excludedProperty))
            {
                schema.Properties.Remove(excludedProperty);
            }
        }

        Properties.TryAdd(context.Type, schema.Properties);
    }
}