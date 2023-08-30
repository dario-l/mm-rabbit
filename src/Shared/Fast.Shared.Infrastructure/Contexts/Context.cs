using System.Diagnostics;
using Fast.Shared.Abstractions.Contexts;

namespace Fast.Shared.Infrastructure.Contexts;

internal sealed class Context : IContext
{
    public string ActivityId { get; }
    public string? TraceId { get; }
    public string? UserId { get; }

    public Context()
    {
        ActivityId = Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString();
        TraceId = string.Empty;
    }

    public Context(string activityId, string? traceId = default, string? userId = default)
    {
        ActivityId = activityId;
        TraceId = traceId;
        UserId = userId;
    }
}