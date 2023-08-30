using Fast.Rides.Domain.Exceptions;
using Fast.Rides.Domain.ValueObjects;

namespace Fast.Rides.Domain.Entities;

internal sealed class RideRequest
{
    public RideRequestId Id { get; private set; } = null!;
    public CustomerId CustomerId { get; private set; } = null!;
    public Route Route { get; private set; } = null!;
    public Price Price { get; private set; } = null!;
    public DateTime? ConfirmedAt { get; private set; }
    public bool Confirmed => ConfirmedAt.HasValue;

    private RideRequest()
    {
    }

    private RideRequest(RideRequestId id, CustomerId customerId, Route route, Price price)
    {
        Id = id;
        CustomerId = customerId;
        Route = route;
        Price = price;
    }

    public void Confirm(DateTime confirmedAt)
    {
        if (Confirmed)
        {
            throw new RideRequestAlreadyConfirmedException(Id.Value);
        }

        ConfirmedAt = confirmedAt;
    }

    public static RideRequest Create(RideRequestId id, CustomerId customerId, Route route, Price price)
        => new(id, customerId, route, price);
}