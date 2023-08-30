using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Fast.Shared.Abstractions.Modules;
using Microsoft.Extensions.Configuration;

namespace Fast.Shared.Infrastructure.Modules;

public static class ModuleLoader
{
    public static Assembly[] LoadAssemblies(IConfiguration configuration)
    {
        var appOptions = configuration.BindAppOptions();
        var projectNamespace = appOptions.ModuleLoader.ProjectNamespace;
        if (string.IsNullOrWhiteSpace(projectNamespace))
        {
            throw new InvalidOperationException("Missing project namespace setting in module loader.");
        }

        if (!projectNamespace.EndsWith("."))
        {
            projectNamespace = $"{projectNamespace}.";
        }
        
        var excludedNamespaces = new HashSet<string>(appOptions.ModuleLoader.ExcludedNamespaces ?? Enumerable.Empty<string>());
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll").ToList();
        var disabledModules = new List<string>();
        foreach (var file in files)
        {
            var name = file.Split(Path.DirectorySeparatorChar).Last();
            if (!name.Contains(projectNamespace))
            {
                continue;
            }

            if (excludedNamespaces.Any(x => name.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
            {
                continue;
            }

            var moduleName = name.Split(projectNamespace)[1].Split(".")[0].ToLowerInvariant();
            var enabled = configuration.GetValue<bool>($"{moduleName}:module:enabled");
            if (!enabled)
            {
                disabledModules.Add(file);
            }
        }

        foreach (var disabledModule in disabledModules)
        {
            files.Remove(disabledModule);
        }
            
        files.ForEach(x => assemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(x))));

        return assemblies.ToArray();
    }

    internal static IModule[] LoadModules(IEnumerable<Assembly> assemblies)
        => assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(IModule).IsAssignableFrom(x) && !x.IsInterface)
            .OrderBy(x => x.Name)
            .Select(Activator.CreateInstance)
            .Cast<IModule>()
            .ToArray();
}