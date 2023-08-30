using Fast.Pricings.Core.Clients.DTO;

namespace Fast.Pricings.Core.Clients;

internal interface IDriversClient
{
    Task<DriverDto[]> GetAllAsync();
}