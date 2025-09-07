using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Seeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "CarNumber", "LicenseNumber", "UserId" },
                values: new object[] { "D1", "CAR123", "LIC123", "1d9f8228-d327-4d93-9cfc-02835fd7bbf4" });

            migrationBuilder.InsertData(
                table: "Passengers",
                columns: new[] { "Id", "UserId" },
                values: new object[] { "P1", "207a1b24-2482-4c8e-8972-bb587f5d8287" });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "City", "Name" },
                values: new object[,]
                {
                    { "S1", "Cairo", "Ramses" },
                    { "S2", "Alexandria", "Sidi Gaber" }
                });

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "Id", "EndStationId", "Price", "StartStationId" },
                values: new object[] { "R1", "S2", 150m, "S1" });

            migrationBuilder.InsertData(
                table: "Trips",
                columns: new[] { "Id", "AvailableSeats", "DepartureTime", "DriverId", "RouteId" },
                values: new object[] { "T1", 5, new DateTime(2025, 9, 4, 23, 5, 21, 432, DateTimeKind.Utc).AddTicks(8499), "D1", "R1" });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingDate", "PassengerId", "Status", "TripId" },
                values: new object[] { "B1", new DateTime(2025, 9, 4, 21, 5, 21, 432, DateTimeKind.Utc).AddTicks(8534), "P1", 0, "T1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B1");

            migrationBuilder.DeleteData(
                table: "Passengers",
                keyColumn: "Id",
                keyValue: "P1");

            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: "T1");

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: "D1");

            migrationBuilder.DeleteData(
                table: "Routes",
                keyColumn: "Id",
                keyValue: "R1");

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: "S1");

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: "S2");
        }
    }
}
