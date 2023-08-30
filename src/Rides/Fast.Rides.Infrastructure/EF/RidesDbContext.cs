using Fast.Rides.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fast.Rides.Infrastructure.EF;

internal sealed class RidesDbContext : DbContext
{
    public DbSet<Ride> Rides { get; set; } = null!;
    public DbSet<RideRequest> RideRequests { get; set; } = null!;

    public RidesDbContext(DbContextOptions<RidesDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("rides");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}