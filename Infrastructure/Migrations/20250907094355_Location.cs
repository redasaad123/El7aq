using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Location : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "security",
                table: "UserToken",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                schema: "security",
                table: "UserToken",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                schema: "security",
                table: "UserLogin",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                schema: "security",
                table: "UserLogin",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "Drivers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Long",
                table: "Drivers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B1",
                column: "BookingDate",
                value: new DateTime(2025, 9, 7, 9, 43, 53, 984, DateTimeKind.Utc).AddTicks(3830));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B2",
                column: "BookingDate",
                value: new DateTime(2025, 9, 7, 9, 43, 53, 984, DateTimeKind.Utc).AddTicks(3856));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B3",
                column: "BookingDate",
                value: new DateTime(2025, 9, 7, 9, 43, 53, 984, DateTimeKind.Utc).AddTicks(3835));

            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: "D1",
                columns: new[] { "Lat", "Long" },
                values: new object[] { 0.0, 0.0 });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "CarNumber", "Lat", "LicenseNumber", "Long", "UserId" },
                values: new object[,]
                {
                    { "D4", "CAR321", 31.039999999999999, "LIC321", 31.378499999999999, "0825e731-d92c-4e82-bd5a-f25d3b4c20b7" },
                    { "D5", "CAR654", 31.048500000000001, "LIC654", 31.376999999999999, "045fe3f3-6d2a-411f-8797-148a712adcff" },
                    { "D6", "CAR987", 31.033000000000001, "LIC987", 31.384, "1567814e-d3e3-47aa-92d7-9fb944e24d44" }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "PassengerProfileId", "UserId" },
                values: new object[,]
                {
                    { "6e0263df-9dfc-49ac-b0fd-d0360ea52d02", new DateTime(2025, 9, 7, 9, 38, 53, 984, DateTimeKind.Utc).AddTicks(3960), false, "ay 7aga 22222.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "8b7931a6-d7e9-427b-9cf2-b45d43ca6f16", new DateTime(2025, 9, 7, 10, 3, 53, 984, DateTimeKind.Utc).AddTicks(3957), false, "ay 7aga 1111.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "9acac23e-d282-4477-95e9-7892b27224cc", new DateTime(2025, 9, 7, 9, 43, 53, 984, DateTimeKind.Utc).AddTicks(3947), true, "Welcome to El7aq! Your account was created successfully.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "af8b5efd-5b06-455f-a324-fc4c5e216f4b", new DateTime(2025, 9, 7, 9, 28, 53, 984, DateTimeKind.Utc).AddTicks(3951), false, "Your first booking is pending confirmation.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: "D4");

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: "D5");

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: "D6");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "6e0263df-9dfc-49ac-b0fd-d0360ea52d02");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "8b7931a6-d7e9-427b-9cf2-b45d43ca6f16");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "9acac23e-d282-4477-95e9-7892b27224cc");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "af8b5efd-5b06-455f-a324-fc4c5e216f4b");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Long",
                table: "Drivers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "security",
                table: "UserToken",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                schema: "security",
                table: "UserToken",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                schema: "security",
                table: "UserLogin",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                schema: "security",
                table: "UserLogin",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B1",
                column: "BookingDate",
                value: new DateTime(2025, 9, 6, 22, 9, 18, 389, DateTimeKind.Utc).AddTicks(1682));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B2",
                column: "BookingDate",
                value: new DateTime(2025, 9, 6, 22, 9, 18, 389, DateTimeKind.Utc).AddTicks(1706));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B3",
                column: "BookingDate",
                value: new DateTime(2025, 9, 6, 22, 9, 18, 389, DateTimeKind.Utc).AddTicks(1689));

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
