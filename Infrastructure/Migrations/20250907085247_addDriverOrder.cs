using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addDriverOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B1");

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B2");

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B3");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "482edf44-3288-4850-ba72-9b55b14e21f6");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "6ec17115-61cd-4ce1-bb49-8a486ef15cef");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "71616057-539c-4687-b308-979d0261056d");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "9c25c18c-c6d2-44aa-9a08-a044c8ab4223");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingDate", "PassengerId", "Status", "TripId" },
                values: new object[,]
                {
                    { "B1", new DateTime(2025, 9, 6, 22, 9, 18, 389, DateTimeKind.Utc).AddTicks(1682), "P1", 0, "T1" },
                    { "B2", new DateTime(2025, 9, 6, 22, 9, 18, 389, DateTimeKind.Utc).AddTicks(1706), "P1", 0, "T1" },
                    { "B3", new DateTime(2025, 9, 6, 22, 9, 18, 389, DateTimeKind.Utc).AddTicks(1689), "P1", 0, "T1" }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "PassengerProfileId", "UserId" },
                values: new object[,]
                {
                    { "482edf44-3288-4850-ba72-9b55b14e21f6", new DateTime(2025, 9, 6, 22, 29, 18, 389, DateTimeKind.Utc).AddTicks(1781), false, "ay 7aga 1111.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "6ec17115-61cd-4ce1-bb49-8a486ef15cef", new DateTime(2025, 9, 6, 22, 9, 18, 389, DateTimeKind.Utc).AddTicks(1772), true, "Welcome to El7aq! Your account was created successfully.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "71616057-539c-4687-b308-979d0261056d", new DateTime(2025, 9, 6, 22, 4, 18, 389, DateTimeKind.Utc).AddTicks(1783), false, "ay 7aga 22222.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "9c25c18c-c6d2-44aa-9a08-a044c8ab4223", new DateTime(2025, 9, 6, 21, 54, 18, 389, DateTimeKind.Utc).AddTicks(1775), false, "Your first booking is pending confirmation.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" }
                });
        }
    }
}
