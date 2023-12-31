﻿// <auto-generated />
using System;
using Fast.Rides.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fast.Rides.Infrastructure.EF.Migrations
{
    [DbContext(typeof(RidesDbContext))]
    partial class RidesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("rides")
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Fast.Rides.Domain.Entities.Ride", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<long>("CustomerId")
                        .HasColumnType("bigint");

                    b.Property<long>("DriverId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("FinishedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<long>("RequestId")
                        .HasColumnType("bigint");

                    b.Property<long?>("RideRequestId")
                        .HasColumnType("bigint");

                    b.Property<string>("Route")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RequestId")
                        .IsUnique();

                    b.HasIndex("RideRequestId");

                    b.ToTable("Rides", "rides");
                });

            modelBuilder.Entity("Fast.Rides.Domain.Entities.RideRequest", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("ConfirmedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("CustomerId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<string>("Route")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("RideRequests", "rides");
                });

            modelBuilder.Entity("Fast.Rides.Domain.Entities.Ride", b =>
                {
                    b.HasOne("Fast.Rides.Domain.Entities.RideRequest", null)
                        .WithMany()
                        .HasForeignKey("RideRequestId");
                });
#pragma warning restore 612, 618
        }
    }
}
