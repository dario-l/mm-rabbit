using Fast.Rides.Application.DTO;
using Fast.Rides.Application.Queries;
using Fast.Rides.Domain.ValueObjects;
using Fast.Shared.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace Fast.Rides.Infrastructure.EF.Queries.Handlers;

internal sealed class GetRideRequestHandler : IQueryHandler<GetRideRequest, RideRequestDto?>
{
    private readonly RidesDbContext _dbContext;

    public GetRideRequestHandler(RidesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RideRequestDto?> HandleAsync(GetRideRequest query, CancellationToken cancellationToken = default)
        => _dbContext.RideRequests
            .Where(x => x.Id == new RideRequestId(query.RideRequestId))
            .Select(x => new RideRequestDto(x.Id.Value, x.CustomerId.Value,
                new RouteDto(x.Route.From.Value, x.Route.To.Value), x.Price.Value,
                x.Confirmed ? "confirmed" : "unconfirmed"))
            .SingleOrDefaultAsync(cancellationToken);
}