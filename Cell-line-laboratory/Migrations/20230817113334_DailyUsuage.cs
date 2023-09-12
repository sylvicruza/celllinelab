using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cell_line_laboratory.Migrations
{
    public partial class DailyUsuage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyUsage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CellLineCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Usage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyUsage", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "1f7621b3-1967-48d3-bf1a-34c07d99c079", new DateTime(2023, 8, 17, 12, 33, 34, 700, DateTimeKind.Local).AddTicks(2555), "AQAAAAEAACcQAAAAEHh7nSsZWI6HN/nZzvPz8CqahBILqMAQ4dORR0EAWo3VPhmhzjKYi0iE9S0zRd7jww==", "d5683b60-ce2d-494a-b402-7071146644e5" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyUsage");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "76a68211-d513-4539-abe8-8db8e0c02329", new DateTime(2023, 8, 17, 12, 29, 10, 950, DateTimeKind.Local).AddTicks(1459), "AQAAAAEAACcQAAAAEE1LabBSx0fazXj1C/Qkh18GO0mXsoJc3vQrlWPKCsThOjCddTfQ0AnYt2n3wklN6Q==", "cad6d0f8-0bd4-4f5b-9175-aebd7ee869db" });
        }
    }
}
