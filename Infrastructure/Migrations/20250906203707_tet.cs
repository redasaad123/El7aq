using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class tet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var userid = "ae065960-4630-407c-9188-4fd5fd89ee6e";
            migrationBuilder.InsertData(
                schema: "security",
                table: "Users",
                columns: new[]
                {
                    "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
                    "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
                    "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount",
                    "FirstName", "LastName"
                 },
                values: new object[]
                {
                    userid, // Id
                    "admin@system.com",        // UserName
                    "ADMIN@SYSTEM.COM",        // NormalizedUserName
                    "admin@system.com",        // Email
                    "ADMIN@SYSTEM.COM",        // NormalizedEmail
                    false,                      // EmailConfirmed
                    "AQAAAAEAACcQAAAAEGIgQfyMlU6Q8UvVgKNhhfRrZIaptQv526F2sdEeyy07gPEyDyN+yY2Ep1T5V6Cuiw==",
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    false, false, true, 0,
                    "Basmala",
                    "mohammed"
                }
                );

            migrationBuilder.InsertData(
                schema: "security",
                table: "UserRole",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] {userid, "511a45e6-1185-482c-a382-a52f7fa942a2" });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [security].[UserRoles] WHERE UserId = 'ae065960-4630-407c-9188-4fd5fd89ee6e'");
            migrationBuilder.Sql("DELETE FROM [security].[Users] WHERE Id = 'ae065960-4630-407c-9188-4fd5fd89ee6e'");
            migrationBuilder.Sql("DELETE FROM [security].[Roles] WHERE Id = '511a45e6-1185-482c-a382-a52f7fa942a2'");
        }
    }
}
