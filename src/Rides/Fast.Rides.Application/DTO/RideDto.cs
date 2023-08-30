namespace Fast.Rides.Application.DTO;

internal sealed record RideDto(long Id, long CustomerId, long DriverId, RouteDto Route, decimal Price, string Status);