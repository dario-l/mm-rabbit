using Fast.Services.Drivers.Application.Events.Out;
using Fast.Services.Drivers.DataAccess;
using Fast.Shared.Abstractions.Events;
using Fast.Shared.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Fast.Services.Drivers.Application.Events.In.Handlers;

internal sealed class RideFinishedHandler : IEventHandler<RideFinished>
{
    private readonly DriversDbContext _dbContext;
    private readonly IMessageBroker _messageBroker;

    public RideFinishedHandler(DriversDbContext dbContext, IMessageBroker messageBroker)
    {
        _dbContext = dbContext;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(RideFinished @event, CancellationToken cancellationToken = default)
    {
        var driver = await _dbContext.Drivers.SingleAsync(x => x.Id == @event.DriverId, cancellationToken);
        if (driver.Available)
        {
            return;
        }

        driver.Available = true;
        _dbContext.Drivers.Update(driver);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _messageBroker.SendAsync(new DriverAvailable(@event.DriverId), cancellationToken);
    }
}