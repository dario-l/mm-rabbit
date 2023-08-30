using Fast.Drivers.Application.Events.Out;
using Fast.Drivers.DataAccess;
using Fast.Drivers.DataAccess.Entities;
using Fast.Shared.Abstractions.Commands;
using Fast.Shared.Abstractions.Messaging;

namespace Fast.Drivers.Application.Commands.Handlers;

internal sealed class RegisterDriverHandler : ICommandHandler<RegisterDriver>
{
    private readonly DriversDbContext _dbContext;
    private readonly IMessageBroker _messageBroker;

    public RegisterDriverHandler(DriversDbContext dbContext, IMessageBroker messageBroker)
    {
        _dbContext = dbContext;
        _messageBroker = messageBroker;
    }
    
    public async Task HandleAsync(RegisterDriver command, CancellationToken cancellationToken = default)
    {
        var driver = new Driver
        {
            Id = command.DriverId,
            Name = command.Name,
            Available = true
        };
        await _dbContext.AddAsync(driver, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _messageBroker.SendAsync(new DriverRegistered(command.DriverId), cancellationToken);
    }
}