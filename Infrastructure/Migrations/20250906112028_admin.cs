using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AdminMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("Bookings", "Id", "B1");
            migrationBuilder.DeleteData("Bookings", "Id", "B2");
            migrationBuilder.DeleteData("Bookings", "Id", "B3");

            migrationBuilder.DeleteData("Notifications", "Id", "4e5aa000-99cd-43db-9cb5-3d078583a281");
            migrationBuilder.DeleteData("Notifications", "Id", "9619f20d-6be9-4346-99f6-a0989af73e45");
            migrationBuilder.DeleteData("Notifications", "Id", "a16e5619-72e7-4629-8081-37abc4347cf5");
            migrationBuilder.DeleteData("Notifications", "Id", "bac07b9e-4cfd-4d7d-8f65-62465be06a84");

            migrationBuilder.DeleteData("Trips", "Id", "T1");
            migrationBuilder.DeleteData("Trips", "Id", "T2");
            migrationBuilder.DeleteData("Trips", "Id", "T3");

            migrationBuilder.DeleteData("Stations", "Id", "S1");
            migrationBuilder.DeleteData("Stations", "Id", "S2");
            migrationBuilder.DeleteData("Stations", "Id", "S3");

            migrationBuilder.DeleteData("Routes", "Id", "R1");

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
            migrationBuilder.InsertData(
                table:"Roles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    { Guid.NewGuid().ToString(), "Admin", "ADMIN", Guid.NewGuid().ToString() },
                    { Guid.NewGuid().ToString(), "Driver", "DRIVER", Guid.NewGuid().ToString() },
                    { Guid.NewGuid().ToString(), "Passenger", "PASSENGER", Guid.NewGuid().ToString() }
                }  
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [Roles] WHERE [Name] IN ('Admin', 'Driver', 'Passenger')");
            migrationBuilder.DeleteData("Notifications", "Id", "684966c2-802e-4502-ad40-e5e219ed1bba");
            migrationBuilder.DeleteData("Notifications", "Id", "a0629f67-1b6b-48f9-86ed-93bdb165420f");
            migrationBuilder.DeleteData("Notifications", "Id", "c256976f-8329-49dd-89d5-8de41f8a3b1e");
            migrationBuilder.DeleteData("Notifications", "Id", "e7463b17-c048-416e-98ac-710c1402a661");

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "City", "Name" },
                values: new object[,]
                {
                    { "S1", "Cairo", "Ramses" },
                    { "S2", "Alexandria", "Sidi Gaber" },
                    { "S3", "Alexandria", "Downtown Hub" }
                });

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "Id", "EndStationId", "Price", "StartStationId" },
                values: new object[] { "R1", "S2", 150m, "S1" });

            migrationBuilder.InsertData(
                table: "Trips",
                columns: new[] { "Id", "AvailableSeats", "DriverId", "RouteId" },
                values: new object[,]
                {
                    { "T1", 5, "D1", "R1" },
                    { "T2", 8, "D1", "R1" },
                    { "T3", 0, "D1", "R1" }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "TripId", "BookingDate", "PassengerId" },
                values: new object[,]
                {
                    { "B1", "T1", new DateTime(2025, 9, 5, 21, 9, 50, 87, DateTimeKind.Utc).AddTicks(7010), "P1" },
                    { "B2", "T2", new DateTime(2025, 9, 5, 21, 9, 50, 87, DateTimeKind.Utc).AddTicks(7024), "P2" },
                    { "B3", "T3", new DateTime(2025, 9, 5, 21, 9, 50, 87, DateTimeKind.Utc).AddTicks(7013), "P3" }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "PassengerProfileId", "UserId" },
                values: new object[,]
                {
                    { "4e5aa000-99cd-43db-9cb5-3d078583a281", new DateTime(2025, 9, 5, 21, 29, 50, 87, DateTimeKind.Utc).AddTicks(7102), false, "ay 7aga 1111.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "9619f20d-6be9-4346-99f6-a0989af73e45", new DateTime(2025, 9, 5, 21, 9, 50, 87, DateTimeKind.Utc).AddTicks(7087), true, "Welcome to El7aq! Your account was created successfully.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "a16e5619-72e7-4629-8081-37abc4347cf5", new DateTime(2025, 9, 5, 21, 4, 50, 87, DateTimeKind.Utc).AddTicks(7109), false, "ay 7aga 22222.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" },
                    { "bac07b9e-4cfd-4d7d-8f65-62465be06a84", new DateTime(2025, 9, 5, 20, 54, 50, 87, DateTimeKind.Utc).AddTicks(7093), false, "Your first booking is pending confirmation.", null, "95e8cc4e-2c7d-41eb-a292-0c18c66dd2bc" }
                });
        }
    }
}
