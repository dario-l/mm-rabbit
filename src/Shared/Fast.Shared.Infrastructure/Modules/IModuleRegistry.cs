using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fast.Shared.Infrastructure.Modules;

internal interface IModuleRegistry
{
    ModuleRequestRegistration? GetRequestRegistration(string path);
    void AddRequestAction(string path, Type requestType, Type responseType,
        Func<object, CancellationToken, Task<object?>> action);

    IEnumerable<ModuleBroadcastRegistration> GetBroadcastRegistrations(string topic);
    void AddBroadcastAction(Type requestType, Func<object, CancellationToken, Task> action);
}