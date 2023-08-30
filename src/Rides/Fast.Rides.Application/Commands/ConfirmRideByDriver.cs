using Fast.Shared.Abstractions.Commands;

namespace Fast.Rides.Application.Commands;

internal sealed record ConfirmRideByDriver(long RideId, long RideRequestId, long DriverId) : ICommand;