using Fast.Rides.Application.Clients;
using Fast.Rides.Application.Exceptions;
using Fast.Rides.Domain.Entities;
using Fast.Rides.Domain.Repositories;
using Fast.Rides.Domain.ValueObjects;
using Fast.Shared.Abstractions.Commands;

namespace Fast.Rides.Application.Commands.Handlers;

internal sealed class RequestRideHandler : ICommandHandler<RequestRide>
{
    private readonly IRideRequestRepository _rideRequestRepository;
    private readonly IDriversClient _driversClient;
    private readonly IPricingsClient _pricingsClient;

    public RequestRideHandler(IRideRequestRepository rideRequestRepository, IDriversClient driversClient,
        IPricingsClient pricingsClient)
    {
        _rideRequestRepository = rideRequestRepository;
        _driversClient = driversClient;
        _pricingsClient = pricingsClient;
    }
    
    public async Task HandleAsync(RequestRide command, CancellationToken cancellationToken = default)
    {
        var drivers = await _driversClient.GetAllAsync();
        if (!drivers.Any(x => x.Available))
        {
            throw new UnavailableDriversException();
        }
        
        var pricing = await _pricingsClient.CalculateAsync(command.RideRequestId, command.From, command.To);
        var route = new Route(new Location(command.From), new Location(command.To));
        var rideRequest = RideRequest.Create(new RideRequestId(command.RideRequestId), new CustomerId(command.CustomerId),
            route, new Price(pricing));
        await _rideRequestRepository.AddAsync(rideRequest);
    }
}