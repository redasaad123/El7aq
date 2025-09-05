using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PassengerProfileId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Passengers_PassengerProfileId",
                        column: x => x.PassengerProfileId,
                        principalTable: "Passengers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PassengerProfileId",
                table: "Notifications",
                column: "PassengerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B1",
                column: "BookingDate",
                value: new DateTime(2025, 9, 4, 21, 14, 45, 407, DateTimeKind.Utc).AddTicks(4648));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: "B2",
                column: "BookingDate",
                value: new DateTime(2025, 9, 4, 21, 14, 45, 407, DateTimeKind.Utc).AddTicks(4665));

            migrationBuilder.UpdateData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: "T1",
                column: "DepartureTime",
                value: new DateTime(2025, 9, 4, 23, 14, 45, 407, DateTimeKind.Utc).AddTicks(4609));
        }
    }
}
