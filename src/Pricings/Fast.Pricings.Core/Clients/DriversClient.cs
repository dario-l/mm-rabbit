using System.Net.Http.Json;
using Fast.Pricings.Core.Clients.DTO;

namespace Fast.Pricings.Core.Clients;

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