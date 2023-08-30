using Fast.Shared.Abstractions.Exceptions;

namespace Fast.Payments.Api.Exceptions;

internal sealed class PaymentNotFoundException : CustomException
{
    public long Id { get; }

    public PaymentNotFoundException(long id) : base($"Payment with ID: {id} was not found.")
    {
        Id = id;
    }
}