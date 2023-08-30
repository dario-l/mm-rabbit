namespace Fast.Shared.Abstractions.Contexts;

public interface IContext
{
    string ActivityId { get; }
    string? TraceId { get; }
    string? UserId { get; }
}