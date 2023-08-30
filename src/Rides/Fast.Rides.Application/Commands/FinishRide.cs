using Fast.Shared.Abstractions.Commands;

namespace Fast.Rides.Application.Commands;

internal sealed record FinishRide(long RideId) : ICommand;