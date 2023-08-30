using Fast.Services.Drivers.Application.DTO;
using Fast.Services.Drivers.DataAccess;
using Fast.Shared.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace Fast.Services.Drivers.Application.Queries.Handlers;

internal sealed class GetDriverHandler : IQueryHandler<GetDriver, DriverDto?>
{
    private readonly DriversDbContext _dbContext;

    public GetDriverHandler(DriversDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<DriverDto?> HandleAsync(GetDriver query, CancellationToken cancellationToken = default)
        => _dbContext.Drivers
            .Where(x => x.Id == query.DriverId)
            .Select(x => new DriverDto(x.Id, x.Name, x.Available))
            .SingleOrDefaultAsync(cancellationToken);
}