using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fast.Shared.Infrastructure.Modules;

internal sealed class ModuleRegistry : IModuleRegistry
{
    private readonly List<ModuleBroadcastRegistration> _broadcastRegistrations = new();
    private readonly Dictionary<string, ModuleRequestRegistration> _requestRegistrations = new();

    public ModuleRequestRegistration? GetRequestRegistration(string path)
        => _requestRegistrations.TryGetValue(path, out var registration) ? registration : null;
    
    public void AddRequestAction(string path, Type requestType, Type responseType,
        Func<object, CancellationToken, Task<object?>> action)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException("Request path cannot be empty.");
        }

        var registration = new ModuleRequestRegistration(requestType, responseType, action);
        _requestRegistrations.Add(path, registration);
    }
    
    public IEnumerable<ModuleBroadcastRegistration> GetBroadcastRegistrations(string topic)
        => _broadcastRegistrations.Where(x => x.Topic == topic);

    public void AddBroadcastAction(Type requestType, Func<object, CancellationToken, Task> action)
    {
        if (string.IsNullOrWhiteSpace(requestType.Namespace))
        {
            throw new InvalidOperationException("Missing request type namespace.");
        }

        var registration = new ModuleBroadcastRegistration(requestType, action);
        _broadcastRegistrations.Add(registration);
    }
}