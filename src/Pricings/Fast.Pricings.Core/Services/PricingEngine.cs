namespace Fast.Pricings.Core.Services;

internal sealed class PricingEngine : IPricingEngine
{
    private const decimal IncrementalValue = 0.1M;
    private const decimal MinThreshold = 0.5M;
    private const decimal MaxThreshold = 5M;
    private readonly object _lock = new();

    public decimal Multiplier { get; private set; } = 1.0M;
    
    public void IncreaseAvailableDrivers()
    {
        lock (_lock)
        {
            if (Multiplier - IncrementalValue < MinThreshold)
            {
                return;
            }

            Multiplier -= IncrementalValue;
        }
    }

    public void DecreaseAvailableDrivers()
    {
        lock (_lock)
        {
            if (Multiplier + IncrementalValue > MaxThreshold)
            {
                return;
            }

            Multiplier += IncrementalValue;
        }
    }
}