using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_users_UserId",
                table: "Drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_users_UserId",
                table: "Passengers");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Bookings_BookingId1",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_users_UserId",
                table: "Staffs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaim_users_UserId",
                schema: "security",
                table: "UserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogin_users_UserId",
                schema: "security",
                table: "UserLogin");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_users_UserId",
                schema: "security",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserToken_users_UserId",
                schema: "security",
                table: "UserToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                schema: "security",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_Payments_BookingId1",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BookingId1",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "security",
                newName: "Users",
                newSchema: "security");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "PassengerId",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                schema: "security",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PassengerId",
                table: "Payments",
                column: "PassengerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Users_UserId",
                table: "Drivers",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_Users_UserId",
                table: "Passengers",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Passengers_PassengerId",
                table: "Payments",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Users_UserId",
                table: "Staffs",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaim_Users_UserId",
                schema: "security",
                table: "UserClaim",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogin_Users_UserId",
                schema: "security",
                table: "UserLogin",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Users_UserId",
                schema: "security",
                table: "UserRole",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserToken_Users_UserId",
                schema: "security",
                table: "UserToken",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Users_UserId",
                table: "Drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_Users_UserId",
                table: "Passengers");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Passengers_PassengerId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Users_UserId",
                table: "Staffs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaim_Users_UserId",
                schema: "security",
                table: "UserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogin_Users_UserId",
                schema: "security",
                table: "UserLogin");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Users_UserId",
                schema: "security",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserToken_Users_UserId",
                schema: "security",
                table: "UserToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                schema: "security",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PassengerId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PassengerId",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "security",
                newName: "users",
                newSchema: "security");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Payments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BookingId1",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                schema: "security",
                table: "users",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId1",
                table: "Payments",
                column: "BookingId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_users_UserId",
                table: "Drivers",
                column: "UserId",
                principalSchema: "security",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_users_UserId",
                table: "Passengers",
                column: "UserId",
                principalSchema: "security",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Bookings_BookingId1",
                table: "Payments",
                column: "BookingId1",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_users_UserId",
                table: "Staffs",
                column: "UserId",
                principalSchema: "security",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaim_users_UserId",
                schema: "security",
                table: "UserClaim",
                column: "UserId",
                principalSchema: "security",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogin_users_UserId",
                schema: "security",
                table: "UserLogin",
                column: "UserId",
                principalSchema: "security",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_users_UserId",
                schema: "security",
                table: "UserRole",
                column: "UserId",
                principalSchema: "security",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserToken_users_UserId",
                schema: "security",
                table: "UserToken",
                column: "UserId",
                principalSchema: "security",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
