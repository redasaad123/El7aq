using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update password hash for Ahmed Qassem user
            migrationBuilder.UpdateData(
                schema: "security",
                table: "Users",
                keyColumn: "Id",
                keyValue: "ahmed-qassem-new-user-id",
                columns: new[] { "PasswordHash" },
                values: new object[] { "AQAAAAIAAYagAAAAEAKCxhVf87Dlq0Y+PPFu+qvP0NzOTMwuYbXwti/M4ATV97fvgOJ+zsN18cqdzSVFWg==" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert password hash to previous value
            migrationBuilder.UpdateData(
                schema: "security",
                table: "Users",
                keyColumn: "Id",
                keyValue: "ahmed-qassem-new-user-id",
                columns: new[] { "PasswordHash" },
                values: new object[] { "AQAAAAIAAYagAAAAENsdiWbDERFrvCpivy2RzoTByhM9WPcl9WpwVsO5t0kTPDv0weGhCByRr7yo7s2oKQ==" }
            );
        }
    }
}
