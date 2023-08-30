using Fast.Rides.Domain.Entities;
using Fast.Rides.Domain.ValueObjects;

namespace Fast.Rides.Domain.Repositories;

internal interface IRideRequestRepository
{
    Task<RideRequest?> GetAsync(RideRequestId id);
    Task AddAsync(RideRequest request);
    Task UpdateAsync(RideRequest request);
}