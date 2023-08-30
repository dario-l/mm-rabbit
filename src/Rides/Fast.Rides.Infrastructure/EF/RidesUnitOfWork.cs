using Fast.Shared.Infrastructure.Postgres;

namespace Fast.Rides.Infrastructure.EF;

internal sealed class RidesUnitOfWork : PostgresUnitOfWork<RidesDbContext>
{
    public RidesUnitOfWork(RidesDbContext dbContext) : base(dbContext)
    {
    }
}