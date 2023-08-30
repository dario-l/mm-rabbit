using Fast.Drivers.Application.DTO;
using Fast.Shared.Abstractions.Queries;

namespace Fast.Drivers.Application.Queries;

internal sealed record GetDriver(long DriverId) : IQuery<DriverDto?>;