using Fast.Shared.Abstractions.Commands;

namespace Fast.Drivers.Application.Commands;

internal sealed record RegisterDriver(long DriverId, string Name) : ICommand;