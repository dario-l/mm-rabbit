using Fast.Rides.Domain.Exceptions;

namespace Fast.Rides.Domain.ValueObjects;

internal record Price
{
    public decimal Value { get; }

    public Price(decimal value)
    {
        if (value < 0)
        {
            throw new InvalidPriceException(value);
        }
        
        Value = value;
    }
};