using Fast.Rides.Domain.Entities;
using Fast.Rides.Domain.Repositories;
using Fast.Rides.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Fast.Rides.Infrastructure.EF.Repositories;

internal sealed class RideRequestRepository : IRideRequestRepository
{
    private readonly RidesDbContext _dbContext;
    private readonly DbSet<RideRequest> _requests;

    public RideRequestRepository(RidesDbContext dbContext)
    {
        _dbContext = dbContext;
        _requests = dbContext.RideRequests;
    }

    public Task<RideRequest?> GetAsync(RideRequestId id) => _requests.SingleOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(RideRequest request)
    {
        await _requests.AddAsync(request);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(RideRequest request)
    {
        _requests.Update(request);
        await _dbContext.SaveChangesAsync();
    }
}