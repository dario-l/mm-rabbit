using Fast.Rides.Application.Events.Out;
using Fast.Rides.Application.Exceptions;
using Fast.Rides.Domain.Entities;
using Fast.Rides.Domain.Repositories;
using Fast.Rides.Domain.ValueObjects;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Rides.Application.Commands.Handlers;

internal sealed class ConfirmRideByDriverHandler : ICommandHandler<ConfirmRideByDriver>
{
    private readonly IRideRequestRepository _rideRequestRepository;
    private readonly IRideRepository _rideRepository;
    private readonly IMessageBroker _messageBroker;

    public ConfirmRideByDriverHandler(IRideRequestRepository rideRequestRepository, IRideRepository rideRepository,
        IMessageBroker messageBroker)
    {
        _rideRequestRepository = rideRequestRepository;
        _rideRepository = rideRepository;
        _messageBroker = messageBroker;
    }
    
    public async Task HandleAsync(ConfirmRideByDriver command, CancellationToken cancellationToken = default)
    {
        var rideRequest = await _rideRequestRepository.GetAsync(new RideRequestId(command.RideRequestId));
        if (rideRequest is null)
        {
            throw new RideRequestNotFoundException(command.RideRequestId);
        }
        
        if (await _rideRepository.GetByRequestAsync(new RideRequestId(command.RideRequestId)) is not null)
        {
            throw new RideAlreadyExistsException(command.RideRequestId);
        }

        var ride = Ride.Create(new RideId(command.RideId), new DriverId(command.DriverId), rideRequest);
        await _rideRepository.AddAsync(ride);
        await _messageBroker.SendAsync(new RideConfirmed(ride.Id.Value, ride.CustomerId.Value, ride.DriverId.Value,
            ride.Price.Value), cancellationToken);
    }
}