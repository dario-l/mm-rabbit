using System;
using Fast.Shared.Abstractions.Time;

namespace Fast.Shared.Infrastructure.Time;

internal sealed class UtcClock : IClock
{
    public DateTime CurrentDate() => DateTime.UtcNow;
}