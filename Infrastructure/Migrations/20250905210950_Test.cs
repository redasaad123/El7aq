using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                value: new DateTime(2025, 9, 5, 21, 9, 50, 87, DateTimeKind.Utc).AddTicks(7010));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B2",
                column: "BookingDate",
                value: new DateTime(2025, 9, 5, 21, 9, 50, 87, DateTimeKind.Utc).AddTicks(7024));

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingDate", "PassengerId", "Status", "TripId" },
                values: new object[] { "B3", new DateTime(2025, 9, 5, 21, 9, 50, 87, DateTimeKind.Utc).AddTicks(7013), "P1", 0, "T1" });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "PassengerProfileId", "UserId" },
                values: new object[,]
                {
                    { "4e5aa000-99cd-43db-9cb5-3d078583a281", new DateTime(2025, 9, 5, 21, 29, 50, 87, DateTimeKind.Utc).AddTicks(7102), false, "ay 7aga 1111.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "9619f20d-6be9-4346-99f6-a0989af73e45", new DateTime(2025, 9, 5, 21, 9, 50, 87, DateTimeKind.Utc).AddTicks(7087), true, "Welcome to El7aq! Your account was created successfully.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "a16e5619-72e7-4629-8081-37abc4347cf5", new DateTime(2025, 9, 5, 21, 4, 50, 87, DateTimeKind.Utc).AddTicks(7109), false, "ay 7aga 22222.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "bac07b9e-4cfd-4d7d-8f65-62465be06a84", new DateTime(2025, 9, 5, 20, 54, 50, 87, DateTimeKind.Utc).AddTicks(7093), false, "Your first booking is pending confirmation.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" }
                });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "City", "Name" },
                values: new object[] { "S3", "Alexandria", "Downtown Hub" });

            migrationBuilder.InsertData(
                table: "Trips",
                columns: new[] { "Id", "AvailableSeats", "DriverId", "RouteId" },
                values: new object[,]
                {
                  
                    { "T3", 0, "D1", "R1" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B3");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "4e5aa000-99cd-43db-9cb5-3d078583a281");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "9619f20d-6be9-4346-99f6-a0989af73e45");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "a16e5619-72e7-4629-8081-37abc4347cf5");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "bac07b9e-4cfd-4d7d-8f65-62465be06a84");

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: "S3");

            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: "T2");

            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: "T3");

            migrationBuilder.AddColumn<DateTime>(
                name: "DepartureTime",
                table: "Trips",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
    }
}
