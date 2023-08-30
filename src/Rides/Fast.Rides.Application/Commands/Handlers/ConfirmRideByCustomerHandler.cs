using Fast.Rides.Application.Exceptions;
using Fast.Rides.Domain.Repositories;
using Fast.Rides.Domain.ValueObjects;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Time;

namespace Fast.Rides.Application.Commands.Handlers;

internal sealed class ConfirmRideByCustomerHandler : ICommandHandler<ConfirmRideByCustomer>
{
    private readonly IRideRequestRepository _rideRequestRepository;
    private readonly IClock _clock;

    public ConfirmRideByCustomerHandler(IRideRequestRepository rideRequestRepository, IClock clock)
    {
        _rideRequestRepository = rideRequestRepository;
        _clock = clock;
    }
    
    public async Task HandleAsync(ConfirmRideByCustomer command, CancellationToken cancellationToken = default)
    {
        var rideRequest = await _rideRequestRepository.GetAsync(new RideRequestId(command.RideRequestId));
        if (rideRequest is null)
        {
            throw new RideRequestNotFoundException(command.RideRequestId);
        }
        
        rideRequest.Confirm(_clock.CurrentDate());
        await _rideRequestRepository.UpdateAsync(rideRequest);
    }
}