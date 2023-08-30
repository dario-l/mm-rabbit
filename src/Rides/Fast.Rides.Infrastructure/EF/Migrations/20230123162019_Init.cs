using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fast.Rides.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "rides");

            migrationBuilder.CreateTable(
                name: "RideRequests",
                schema: "rides",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Route = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RideRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rides",
                schema: "rides",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    Route = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RideRequestId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rides_RideRequests_RideRequestId",
                        column: x => x.RideRequestId,
                        principalSchema: "rides",
                        principalTable: "RideRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rides_RequestId",
                schema: "rides",
                table: "Rides",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rides_RideRequestId",
                schema: "rides",
                table: "Rides",
                column: "RideRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rides",
                schema: "rides");

            migrationBuilder.DropTable(
                name: "RideRequests",
                schema: "rides");
        }
    }
}
