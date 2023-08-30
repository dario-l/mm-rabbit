using System.Net.Http.Json;
using Fast.Rides.Application.Clients;
using Fast.Rides.Application.Clients.DTO;

namespace Fast.Rides.Infrastructure.Clients;

internal sealed class DriversClient : IDriversClient
{
    private const string Url = "http://localhost:5020";
    private readonly IHttpClientFactory _clientFactory;

    public DriversClient(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<DriverDto[]> GetAllAsync()
    {
        var client = _clientFactory.CreateClient();
        return await client.GetFromJsonAsync<DriverDto[]>($"{Url}/drivers") ?? Array.Empty<DriverDto>();
    }
}