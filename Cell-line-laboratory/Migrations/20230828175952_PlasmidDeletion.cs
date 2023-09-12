using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cell_line_laboratory.Migrations
{
    public partial class PlasmidDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTimestamp",
                table: "PlasmidCollection",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMarkedForDeletion",
                table: "PlasmidCollection",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "cd03b2fa-81c8-4c4d-80a4-57a6d95ed4e8", new DateTime(2023, 8, 28, 18, 59, 52, 26, DateTimeKind.Local).AddTicks(3186), "AQAAAAEAACcQAAAAEDxc55O9o3NG+sCOE1pYMrdFbyvRBbbVJJbTwM4okRa0+9Ai0YDZ3YNo9dHQPpusQA==", "d003b3b7-ef39-4aa5-be14-d08c836a0e2a" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletionTimestamp",
                table: "PlasmidCollection");

            migrationBuilder.DropColumn(
                name: "IsMarkedForDeletion",
                table: "PlasmidCollection");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "1f7621b3-1967-48d3-bf1a-34c07d99c079", new DateTime(2023, 8, 17, 12, 33, 34, 700, DateTimeKind.Local).AddTicks(2555), "AQAAAAEAACcQAAAAEHh7nSsZWI6HN/nZzvPz8CqahBILqMAQ4dORR0EAWo3VPhmhzjKYi0iE9S0zRd7jww==", "d5683b60-ce2d-494a-b402-7071146644e5" });
        }
    }
}
