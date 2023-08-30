using Fast.Rides.Application.Events.Out;
using Fast.Rides.Application.Exceptions;
using Fast.Rides.Domain.Repositories;
using Fast.Rides.Domain.ValueObjects;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;
using Fast.Shared.Abstractions.Time;

namespace Fast.Rides.Application.Commands.Handlers;

internal sealed class FinishRideHandler : ICommandHandler<FinishRide>
{
    private readonly IRideRepository _rideRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;

    public FinishRideHandler(IRideRepository rideRepository, IClock clock, IMessageBroker messageBroker)
    {
        _rideRepository = rideRepository;
        _clock = clock;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(FinishRide command, CancellationToken cancellationToken = default)
    {
        var ride = await _rideRepository.GetAsync(new RideId(command.RideId));
        if (ride is null)
        {
            throw new RideNotFoundException(command.RideId);
        }

        ride.Finish(_clock.CurrentDate());
        await _rideRepository.UpdateAsync(ride);
        await _messageBroker.SendAsync(new RideFinished(ride.Id.Value, ride.CustomerId.Value, ride.DriverId.Value),
            cancellationToken);
    }
}