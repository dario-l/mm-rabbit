using System;

namespace Fast.Shared.Abstractions.Time;

public interface IClock
{
    DateTime CurrentDate();
}