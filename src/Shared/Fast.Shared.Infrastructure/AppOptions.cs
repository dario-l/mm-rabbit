using System.Collections.Generic;

namespace Fast.Shared.Infrastructure;

internal sealed class AppOptions
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public ModuleLoaderOptions ModuleLoader { get; set; } = new();

    internal sealed class ModuleLoaderOptions
    {
        public string ProjectNamespace { get; set; } = string.Empty;
        public IEnumerable<string>? ExcludedNamespaces { get; set; }
    }
}