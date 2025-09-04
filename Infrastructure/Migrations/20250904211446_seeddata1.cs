using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seeddata1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B1",
                column: "BookingDate",
                value: new DateTime(2025, 9, 4, 21, 14, 45, 407, DateTimeKind.Utc).AddTicks(4648));

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingDate", "PassengerId", "Status", "TripId" },
                values: new object[] { "B2", new DateTime(2025, 9, 4, 21, 14, 45, 407, DateTimeKind.Utc).AddTicks(4665), "P1", 0, "T1" });

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: "T1",
                column: "DepartureTime",
                value: new DateTime(2025, 9, 4, 23, 14, 45, 407, DateTimeKind.Utc).AddTicks(4609));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B2");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B1",
                column: "BookingDate",
                value: new DateTime(2025, 9, 4, 21, 5, 21, 432, DateTimeKind.Utc).AddTicks(8534));

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: "T1",
                column: "DepartureTime",
                value: new DateTime(2025, 9, 4, 23, 5, 21, 432, DateTimeKind.Utc).AddTicks(8499));
        }
    }
}
