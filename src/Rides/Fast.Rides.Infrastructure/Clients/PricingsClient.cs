using Fast.Rides.Application.Clients;
using Fast.Shared.Abstractions.Modules;

namespace Fast.Rides.Infrastructure.Clients;

internal sealed class PricingsClient : IPricingsClient
{
    private readonly IModuleClient _client;

    public PricingsClient(IModuleClient client)
    {
        _client = client;
    }

    public async Task<decimal> CalculateAsync(long pricingId, string from, string to)
        => (await _client.SendAsync<PricingDto>(new CalculatePricing(pricingId, from, to)))!.Pricing;

    private record CalculatePricing(long PricingId, string From, string To);

    private record PricingDto(decimal Pricing);
}