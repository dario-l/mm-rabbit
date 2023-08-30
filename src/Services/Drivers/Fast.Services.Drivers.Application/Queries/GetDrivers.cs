using Fast.Services.Drivers.Application.DTO;
using Fast.Shared.Abstractions.Queries;

namespace Fast.Services.Drivers.Application.Queries;

internal sealed record GetDrivers : IQuery<IEnumerable<DriverDto>>;