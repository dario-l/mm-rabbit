using Fast.Shared.Infrastructure.Postgres;

namespace Fast.Services.Drivers.DataAccess;

internal sealed class DriversUnitOfWork : PostgresUnitOfWork<DriversDbContext>
{
    public DriversUnitOfWork(DriversDbContext dbContext) : base(dbContext)
    {
    }
}