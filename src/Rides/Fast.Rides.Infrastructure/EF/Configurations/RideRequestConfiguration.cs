using Fast.Rides.Domain.Entities;
using Fast.Rides.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fast.Rides.Infrastructure.EF.Configurations;

internal sealed class RideRequestConfiguration : IEntityTypeConfiguration<RideRequest>
{
    public void Configure(EntityTypeBuilder<RideRequest> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever().HasConversion(x => x.Value, x => new RideRequestId(x));
        builder.Property(x => x.CustomerId).HasConversion(x => x.Value, x => new CustomerId(x));
        builder.Property(x => x.Price).HasConversion(x => x.Value, x => new Price(x));
        builder.Property(x => x.Route).HasConversion(x => $"{x.From.Value};{x.To.Value}", x => new Route(Parse(x, 0), Parse(x, 1)));
    }

    private static Location Parse(string value, int index) => new(value.Split(";")[index]);
}