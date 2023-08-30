using Fast.Rides.Application.Clients.DTO;

namespace Fast.Rides.Application.Clients;

internal interface IDriversClient
{
    Task<DriverDto[]> GetAllAsync();
}