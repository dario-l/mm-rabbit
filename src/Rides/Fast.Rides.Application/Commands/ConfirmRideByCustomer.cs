using Fast.Shared.Abstractions.Commands;

namespace Fast.Rides.Application.Commands;

internal sealed record ConfirmRideByCustomer(long RideRequestId) : ICommand;