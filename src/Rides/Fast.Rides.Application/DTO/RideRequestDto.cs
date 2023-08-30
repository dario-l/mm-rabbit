namespace Fast.Rides.Application.DTO;

internal sealed record RideRequestDto(long Id, long CustomerId, RouteDto Route, decimal Price, string Status);