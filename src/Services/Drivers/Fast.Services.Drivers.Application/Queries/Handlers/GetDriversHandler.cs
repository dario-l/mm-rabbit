using Fast.Services.Drivers.Application.DTO;
using Fast.Services.Drivers.DataAccess;
using Fast.Shared.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace Fast.Services.Drivers.Application.Queries.Handlers;

internal sealed class GetDriversHandler : IQueryHandler<GetDrivers, IEnumerable<DriverDto>>
{
    private readonly DriversDbContext _dbContext;

    public GetDriversHandler(DriversDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<DriverDto>> HandleAsync(GetDrivers query, CancellationToken cancellationToken = default)
        => await _dbContext.Drivers
            .Select(x => new DriverDto(x.Id, x.Name, x.Available))
            .ToListAsync(cancellationToken);
}