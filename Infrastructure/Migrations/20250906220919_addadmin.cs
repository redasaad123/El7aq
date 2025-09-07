using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addadmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO [security].[Users] 
                ([Id], [FirstName], [LastName], [AccessFailedCount], [ConcurrencyStamp], [Email], [EmailConfirmed], [LockoutEnabled], [LockoutEnd], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName]) 
                VALUES 
                (N'ae065960-4630-407c-9188-4fd5fd89ee6e', 
                 N'Basmala', 
                 N'mohammed', 
                 0, 
                 N'3d43f521-386b-43c2-8cfe-c20e9f09fb5d', 
                 N'admin@system.com', 
                 0, 
                 1, 
                 NULL, 
                 N'ADMIN@SYSTEM.COM', 
                 N'ADMIN@SYSTEM.COM', 
                 N'AQAAAAEAACcQAAAAELDE6Jqyzzxm0vW5FPTTN0y5P+qKo5JtFTXeMQHAdg1l/ryw314+0d4vqXmsyGAWFw==', 
                 NULL, 
                 0, 
                 N'a7a7f7c6-ebd9-485f-9090-ff7fae647cfd', 
                 0, 
                 N'admin@system.com')
            ");
           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [security].[Users] WHERE [Id] = 'ae065960-4630-407c-9188-4fd5fd89ee6e'");
        }
    }
}
