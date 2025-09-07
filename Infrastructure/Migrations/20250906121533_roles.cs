using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "Roles",
            columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
            values: new object[,]
            {
                  { Guid.NewGuid().ToString(), "Admin", "ADMIN", Guid.NewGuid().ToString() },
                  { Guid.NewGuid().ToString(), "Driver", "DRIVER", Guid.NewGuid().ToString() },
                  { Guid.NewGuid().ToString(), "Passenger", "PASSENGER", Guid.NewGuid().ToString() }
            },
            schema: "security"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [Roles] WHERE [Name] IN ('Admin', 'Driver', 'Passenger')");
        }
    }
}
