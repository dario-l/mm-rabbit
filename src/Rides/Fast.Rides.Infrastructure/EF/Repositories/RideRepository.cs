using Fast.Rides.Domain.Entities;
using Fast.Rides.Domain.Repositories;
using Fast.Rides.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Fast.Rides.Infrastructure.EF.Repositories;

internal sealed class RideRepository : IRideRepository
{
    private readonly RidesDbContext _dbContext;
    private readonly DbSet<Ride> _rides;

    public RideRepository(RidesDbContext dbContext)
    {
        _dbContext = dbContext;
        _rides = dbContext.Rides;
    }

    public Task<Ride?> GetAsync(RideId id) => _rides.SingleOrDefaultAsync(x => x.Id == id);
    
    public Task<Ride?> GetByRequestAsync(RideRequestId id) => _rides.SingleOrDefaultAsync(x => x.RequestId == id);

    public async Task AddAsync(Ride ride)
    {
        await _rides.AddAsync(ride);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Ride ride)
    {
        _rides.Update(ride);
        await _dbContext.SaveChangesAsync();
    }
}