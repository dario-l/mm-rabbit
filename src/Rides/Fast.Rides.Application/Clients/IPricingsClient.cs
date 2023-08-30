namespace Fast.Rides.Application.Clients;

internal interface IPricingsClient
{
    Task<decimal> CalculateAsync(long pricingId, string from, string to);
}