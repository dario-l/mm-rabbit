using System.Diagnostics;
using Fast.Shared.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;

namespace Fast.Shared.Infrastructure.Contexts;

internal sealed class ContextProvider : IContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ContextAccessor _contextAccessor;

    public ContextProvider(IHttpContextAccessor httpContextAccessor, ContextAccessor contextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _contextAccessor = contextAccessor;
    }

    public IContext Current()
    {
        if (_contextAccessor.Context is not null)
        {
            return _contextAccessor.Context;
        }

        IContext context;
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            var traceId = httpContext.TraceIdentifier;
            var userId = httpContext.User.Identity?.Name;
            context = new Context(Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString(), traceId,
                userId: userId);
        }
        else
        {
            context = new Context(Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString());
        }

        _contextAccessor.Context = context;

        return context;
    }
}