using Fast.Pricings.Core.DTO;

namespace Fast.Pricings.Core.Services;

internal interface IPricer
{
    Task<decimal> CalculateAsync(CalculatePricing command);
}