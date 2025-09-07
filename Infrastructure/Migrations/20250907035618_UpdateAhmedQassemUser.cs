using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAhmedQassemUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update Ahmed Qassem user with correct password hash
            migrationBuilder.UpdateData(
                schema: "security",
                table: "Users",
                keyColumn: "Id",
                keyValue: "ahmed-qassem-new-user-id",
                columns: new[] { "PasswordHash" },
                values: new object[] { "AQAAAAIAAYagAAAAENsdiWbDERFrvCpivy2RzoTByhM9WPcl9WpwVsO5t0kTPDv0weGhCByRr7yo7s2oKQ==" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert Ahmed Qassem user password hash to original
            migrationBuilder.UpdateData(
                schema: "security",
                table: "Users",
                keyColumn: "Id",
                keyValue: "ahmed-qassem-new-user-id",
                columns: new[] { "PasswordHash" },
                values: new object[] { "AQAAAAIAAYagAAAAEBxb0M1n57Q380z00o2Y5JvXflgzprOjzakVoqQUazvHTJliCD/Sj+l3SG6o8HTMPQ==" }
            );
        }
    }
}
