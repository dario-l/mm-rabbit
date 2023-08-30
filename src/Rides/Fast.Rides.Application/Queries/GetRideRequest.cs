using Fast.Rides.Application.DTO;
using Fast.Shared.Abstractions.Queries;

namespace Fast.Rides.Application.Queries;

internal sealed record GetRideRequest(long RideRequestId) : IQuery<RideRequestDto?>;