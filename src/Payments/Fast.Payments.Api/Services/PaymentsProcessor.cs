using Fast.Shared.Abstractions.Identity;

namespace Fast.Payments.Api.Services;

internal sealed class PaymentsProcessor
{
    private readonly Dictionary<long, long> _payments = new();
    private readonly IIdGen _idGen;

    public PaymentsProcessor(IIdGen idGen)
    {
        _idGen = idGen;
    }
    
    public long Create(long rideId)
    {
        var paymentId = _idGen.Create();
        _payments[paymentId] = rideId;
        return paymentId;
    }

    public long? Get(long paymentId) => _payments.TryGetValue(paymentId, out var rideId) ? rideId : null;
}