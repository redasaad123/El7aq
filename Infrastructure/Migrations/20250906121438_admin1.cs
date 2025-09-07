using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class admin1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "684966c2-802e-4502-ad40-e5e219ed1bba");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "a0629f67-1b6b-48f9-86ed-93bdb165420f");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "c256976f-8329-49dd-89d5-8de41f8a3b1e");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "e7463b17-c048-416e-98ac-710c1402a661");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B1",
                column: "BookingDate",
                value: new DateTime(2025, 9, 6, 12, 14, 37, 816, DateTimeKind.Utc).AddTicks(3563));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B2",
                column: "BookingDate",
                value: new DateTime(2025, 9, 6, 12, 14, 37, 816, DateTimeKind.Utc).AddTicks(3584));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B3",
                column: "BookingDate",
                value: new DateTime(2025, 9, 6, 12, 14, 37, 816, DateTimeKind.Utc).AddTicks(3566));

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "PassengerProfileId", "UserId" },
                values: new object[,]
                {
                    { "0ec97a9c-0dac-4507-9c80-4b05a1b8c2db", new DateTime(2025, 9, 6, 12, 9, 37, 816, DateTimeKind.Utc).AddTicks(3667), false, "ay 7aga 22222.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "105d66c8-0d1a-4810-ac98-7b087571be9a", new DateTime(2025, 9, 6, 12, 34, 37, 816, DateTimeKind.Utc).AddTicks(3655), false, "ay 7aga 1111.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "b33278fd-94d1-461a-aba7-4a275d18fd59", new DateTime(2025, 9, 6, 12, 14, 37, 816, DateTimeKind.Utc).AddTicks(3647), true, "Welcome to El7aq! Your account was created successfully.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "bfa869cb-9d06-4e4d-819f-6729b9bc7a5a", new DateTime(2025, 9, 6, 11, 59, 37, 816, DateTimeKind.Utc).AddTicks(3650), false, "Your first booking is pending confirmation.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "0ec97a9c-0dac-4507-9c80-4b05a1b8c2db");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "105d66c8-0d1a-4810-ac98-7b087571be9a");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "b33278fd-94d1-461a-aba7-4a275d18fd59");

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: "bfa869cb-9d06-4e4d-819f-6729b9bc7a5a");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B1",
                column: "BookingDate",
                value: new DateTime(2025, 9, 6, 11, 20, 27, 746, DateTimeKind.Utc).AddTicks(5659));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B2",
                column: "BookingDate",
                value: new DateTime(2025, 9, 6, 11, 20, 27, 746, DateTimeKind.Utc).AddTicks(5689));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B3",
                column: "BookingDate",
                value: new DateTime(2025, 9, 6, 11, 20, 27, 746, DateTimeKind.Utc).AddTicks(5664));

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "PassengerProfileId", "UserId" },
                values: new object[,]
                {
                    { "684966c2-802e-4502-ad40-e5e219ed1bba", new DateTime(2025, 9, 6, 11, 15, 27, 746, DateTimeKind.Utc).AddTicks(5786), false, "ay 7aga 22222.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "a0629f67-1b6b-48f9-86ed-93bdb165420f", new DateTime(2025, 9, 6, 11, 40, 27, 746, DateTimeKind.Utc).AddTicks(5781), false, "ay 7aga 1111.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "c256976f-8329-49dd-89d5-8de41f8a3b1e", new DateTime(2025, 9, 6, 11, 20, 27, 746, DateTimeKind.Utc).AddTicks(5769), true, "Welcome to El7aq! Your account was created successfully.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "e7463b17-c048-416e-98ac-710c1402a661", new DateTime(2025, 9, 6, 11, 5, 27, 746, DateTimeKind.Utc).AddTicks(5774), false, "Your first booking is pending confirmation.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" }
                });
        }
    }
}
