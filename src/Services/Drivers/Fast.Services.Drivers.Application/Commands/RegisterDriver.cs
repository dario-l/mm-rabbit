using Fast.Shared.Abstractions.Commands;

namespace Fast.Services.Drivers.Application.Commands;

internal sealed record RegisterDriver(long DriverId, string Name) : ICommand;