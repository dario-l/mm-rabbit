using Fast.Rides.Application.DTO;
using Fast.Rides.Application.Queries;
using Fast.Rides.Domain.ValueObjects;
using Fast.Shared.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace Fast.Rides.Infrastructure.EF.Queries.Handlers;

internal sealed class GetRideHandler : IQueryHandler<GetRide, RideDto?>
{
    private readonly RidesDbContext _dbContext;

    public GetRideHandler(RidesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RideDto?> HandleAsync(GetRide query, CancellationToken cancellationToken = default)
        => _dbContext.Rides
            .Where(x => x.Id == new RideId(query.RideId))
            .Select(x => new RideDto(x.Id.Value, x.CustomerId.Value, x.DriverId.Value,
                new RouteDto(x.Route.From.Value, x.Route.To.Value), x.Price.Value, x.Finished ? "finished" : "started"))
            .SingleOrDefaultAsync(cancellationToken);
}