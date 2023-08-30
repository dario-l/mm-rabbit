using Fast.Drivers.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fast.Drivers.DataAccess;

internal sealed class DriversDbContext : DbContext
{
    public DbSet<Driver> Drivers { get; set; } = null!;
    
    public DriversDbContext(DbContextOptions<DriversDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("drivers");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}