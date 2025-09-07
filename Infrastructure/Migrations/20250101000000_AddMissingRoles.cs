using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                schema: "security",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    { Guid.NewGuid().ToString(), "Manager", "MANAGER", Guid.NewGuid().ToString() },
                    { Guid.NewGuid().ToString(), "Staff", "STAFF", Guid.NewGuid().ToString() },
                    { Guid.NewGuid().ToString(), "Driver", "DRIVER", Guid.NewGuid().ToString() },
                    { Guid.NewGuid().ToString(), "Passenger", "PASSENGER", Guid.NewGuid().ToString() }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [security].[Roles] WHERE [Name] IN ('Manager', 'Staff')");
        }
    }
}
