using Fast.Shared.Abstractions.Exceptions;

namespace Fast.Rides.Domain.Exceptions;

internal sealed class InvalidPriceException : CustomException
{
    public decimal Price { get; }

    public InvalidPriceException(decimal price) : base($"Price: {price} is invalid.")
    {
        Price = price;
    }
}