using Fast.Shared.Abstractions.Exceptions;

namespace Fast.Pricings.Core.Exceptions;

internal sealed class CannotCalculatePricingException : CustomException
{
    public CannotCalculatePricingException() : base("Cannot calculate pricing.")
    {
    }
}