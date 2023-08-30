using Fast.Shared.Infrastructure.Postgres;

namespace Fast.Drivers.DataAccess;

internal sealed class DriversUnitOfWork : PostgresUnitOfWork<DriversDbContext>
{
    public DriversUnitOfWork(DriversDbContext dbContext) : base(dbContext)
    {
    }
}