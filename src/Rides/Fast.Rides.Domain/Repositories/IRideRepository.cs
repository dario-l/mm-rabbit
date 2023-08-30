using Fast.Rides.Domain.Entities;
using Fast.Rides.Domain.ValueObjects;

namespace Fast.Rides.Domain.Repositories;

internal interface IRideRepository
{
    Task<Ride?> GetAsync(RideId id);
    Task<Ride?> GetByRequestAsync(RideRequestId id);
    Task AddAsync(Ride ride);
    Task UpdateAsync(Ride ride);
}