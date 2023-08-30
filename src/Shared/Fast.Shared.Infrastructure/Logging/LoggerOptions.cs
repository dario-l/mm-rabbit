using System.Collections.Generic;
using Fast.Shared.Infrastructure.Logging.Options;

namespace Fast.Shared.Infrastructure.Logging;

internal sealed class LoggerOptions
{
    public string? Level { get; set; }
    public ConsoleOptions? Console { get; set; }
    public FileOptions? File { get; set; }
    public SeqOptions? Seq { get; set; }
    public IDictionary<string, string>? Overrides { get; set; }
    public IEnumerable<string>? ExcludePaths { get; set; }
    public IEnumerable<string>? ExcludeProperties { get; set; }
    public IDictionary<string, object>? Tags { get; set; }
}