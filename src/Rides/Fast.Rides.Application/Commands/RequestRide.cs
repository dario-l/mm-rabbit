using Fast.Shared.Abstractions.Commands;

namespace Fast.Rides.Application.Commands;

internal sealed record RequestRide(long RideRequestId, long CustomerId, string From, string To) : ICommand;