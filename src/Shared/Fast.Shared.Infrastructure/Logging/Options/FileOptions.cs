namespace Fast.Shared.Infrastructure.Logging.Options;

internal sealed class FileOptions
{
    public bool Enabled { get; set; }
    public string? Path { get; set; }
    public string? Interval { get; set; }
}