using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addSeedDataToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "85b30f65-c328-4eec-be4d-fa38af7b6439");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "ccb7425d-f7b2-4083-a411-7a514d73698b");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B1",
                column: "BookingDate",
                value: new DateTime(2025, 9, 5, 15, 4, 9, 944, DateTimeKind.Utc).AddTicks(1343));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B2",
                column: "BookingDate",
                value: new DateTime(2025, 9, 5, 15, 4, 9, 944, DateTimeKind.Utc).AddTicks(1364));

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "PassengerProfileId", "UserId" },
                values: new object[,]
                {
                    { "002c1411-e7b5-4839-b9d6-a6e2e18e4721", new DateTime(2025, 9, 5, 15, 24, 9, 944, DateTimeKind.Utc).AddTicks(1466), false, "ay 7aga 1111.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "254eb770-a1e0-4935-8c61-b14a858c4313", new DateTime(2025, 9, 5, 14, 59, 9, 944, DateTimeKind.Utc).AddTicks(1481), false, "ay 7aga 22222.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "406cab31-aa40-4ea8-8759-2191de695a09", new DateTime(2025, 9, 5, 14, 49, 9, 944, DateTimeKind.Utc).AddTicks(1460), false, "Your first booking is pending confirmation.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "794497ac-4cc3-455f-b5f2-91e2d26cedc5", new DateTime(2025, 9, 5, 15, 4, 9, 944, DateTimeKind.Utc).AddTicks(1456), true, "Welcome to El7aq! Your account was created successfully.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" }
                });

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: "T1",
                column: "DepartureTime",
                value: new DateTime(2025, 9, 5, 17, 4, 9, 944, DateTimeKind.Utc).AddTicks(1310));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "002c1411-e7b5-4839-b9d6-a6e2e18e4721");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "254eb770-a1e0-4935-8c61-b14a858c4313");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "406cab31-aa40-4ea8-8759-2191de695a09");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "794497ac-4cc3-455f-b5f2-91e2d26cedc5");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B1",
                column: "BookingDate",
                value: new DateTime(2025, 9, 5, 13, 26, 14, 138, DateTimeKind.Utc).AddTicks(6466));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B2",
                column: "BookingDate",
                value: new DateTime(2025, 9, 5, 13, 26, 14, 138, DateTimeKind.Utc).AddTicks(6501));

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "PassengerProfileId", "UserId" },
                values: new object[,]
                {
                    { "85b30f65-c328-4eec-be4d-fa38af7b6439", new DateTime(2025, 9, 5, 13, 26, 14, 138, DateTimeKind.Utc).AddTicks(6598), true, "Welcome to El7aq! Your account was created successfully.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "ccb7425d-f7b2-4083-a411-7a514d73698b", new DateTime(2025, 9, 5, 13, 11, 14, 138, DateTimeKind.Utc).AddTicks(6603), false, "Your first booking is pending confirmation.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" }
                });

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: "T1",
                column: "DepartureTime",
                value: new DateTime(2025, 9, 5, 15, 26, 14, 138, DateTimeKind.Utc).AddTicks(6292));
        }
    }
}
