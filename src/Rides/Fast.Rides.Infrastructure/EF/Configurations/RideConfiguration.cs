using Fast.Rides.Domain.Entities;
using Fast.Rides.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fast.Rides.Infrastructure.EF.Configurations;

internal sealed class RideConfiguration : IEntityTypeConfiguration<Ride>
{
    public void Configure(EntityTypeBuilder<Ride> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever().HasConversion(x => x.Value, x => new RideId(x));
        builder.Property(x => x.RequestId).HasConversion(x => x.Value, x => new RideRequestId(x));
        builder.Property(x => x.CustomerId).HasConversion(x => x.Value, x => new CustomerId(x));
        builder.Property(x => x.DriverId).HasConversion(x => x.Value, x => new DriverId(x));
        builder.Property(x => x.Price).HasConversion(x => x.Value, x => new Price(x));
        builder.Property(x => x.Route).HasConversion(x => $"{x.From.Value};{x.To.Value}", x => new Route(Parse(x, 0), Parse(x, 1)));
        builder.HasOne<RideRequest>().WithMany();
        builder.HasIndex(x => x.RequestId).IsUnique();
    }

    private static Location Parse(string value, int index) => new(value.Split(";")[index]);
}