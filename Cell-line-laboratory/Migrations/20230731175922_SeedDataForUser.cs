using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cell_line_laboratory.Migrations
{
    public partial class SeedDataForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        //    migrationBuilder.InsertData(
        //        table: "User",
        //        columns: new[] { "Id", "CreatedAt", "DeletedAt", "DeletedBy", "Email", "LastUpdatedAt", "Name", "Password", "Role", "Status", "UserType" },
        //        values: new object[] { 1, new DateTime(2023, 7, 31, 18, 59, 22, 802, DateTimeKind.Local).AddTicks(5645), null, null, "superadmin@gmail.com", null, "John Doe", "AQAAAAEAACcQAAAAEL/vopAHaH6l1f8qi85z69y3jGcZf+xQnED6e4XfA53Q6zqGltG7J95NZ0XIfC65hg==", "SuperUser", "Active", "SuperAdmin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "User",
            //    keyColumn: "Id",
            //    keyValue: 1);
        }
    }
}
