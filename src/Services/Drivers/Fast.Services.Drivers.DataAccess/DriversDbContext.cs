using Fast.Services.Drivers.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fast.Services.Drivers.DataAccess;

internal sealed class DriversDbContext : DbContext
{
    public DbSet<Driver> Drivers { get; set; } = null!;
    
    public DriversDbContext(DbContextOptions<DriversDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}