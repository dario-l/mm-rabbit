using Fast.Rides.Domain.Exceptions;
using Fast.Rides.Domain.ValueObjects;

namespace Fast.Rides.Domain.Entities;

internal sealed class Ride
{
    public RideId Id { get; private set; } = null!;
    public RideRequestId RequestId { get; private set; } = null!;
    public CustomerId CustomerId { get; private set; } = null!;
    public DriverId DriverId { get; private set; } = null!;
    public Route Route { get; private set; } = null!;
    public Price Price { get; private set; } = null!;
    public DateTime? FinishedAt { get; private set; }
    public bool Finished => FinishedAt.HasValue;

    private Ride()
    {
    }

    private Ride(RideId id, RideRequestId requestId, CustomerId customerId, DriverId driverId, Route route, Price price)
    {
        Id = id;
        RequestId = requestId;
        CustomerId = customerId;
        DriverId = driverId;
        Route = route;
        Price = price;
    }

    public void Finish(DateTime finishedAt)
    {
        if (Finished)
        {
            throw new RideAlreadyFinishedException(Id.Value);
        }

        FinishedAt = finishedAt;
    }

    public static Ride Create(RideId id, DriverId driverId, RideRequest request)
    {
        if (!request.Confirmed)
        {
            throw new RideRequestUnconfirmedException(request.Id.Value);
        }
        
        return new(id, request.Id, request.CustomerId, driverId, request.Route, request.Price);
    }
}