namespace Fast.Pricings.Core.Services;

internal interface IPricingEngine
{
    decimal Multiplier { get; }
    void IncreaseAvailableDrivers();
    void DecreaseAvailableDrivers();
}