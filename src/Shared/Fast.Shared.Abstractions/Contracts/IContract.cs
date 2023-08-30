using System;
using System.Collections.Generic;

namespace Fast.Shared.Abstractions.Contracts;

public interface IContract
{
    string Producer { get; }
    Type Type { get; }
    public IEnumerable<string> Properties { get; }
}