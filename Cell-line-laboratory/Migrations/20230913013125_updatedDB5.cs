using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cell_line_laboratory.Migrations
{
    public partial class updatedDB5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "EquipmentInventory",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "EquipmentInventory",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "44d0abd4-ec71-439d-9b85-3adc429bcb1a", new DateTime(2023, 9, 13, 2, 31, 25, 568, DateTimeKind.Local).AddTicks(564), "AQAAAAEAACcQAAAAEAJHiW63qHzBPZ7tupePFeUW7gjiBQaroY2QQyyRJvJ3aKd42pFaTxJPeZ5jaPyElg==", "1667e633-a312-4740-873d-89666b21dd3f" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Quantity",
                table: "EquipmentInventory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "EquipmentInventory",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "1cf3787e-88af-428d-b142-fdf1ccf5754a", new DateTime(2023, 9, 12, 23, 56, 13, 773, DateTimeKind.Local).AddTicks(356), "AQAAAAEAACcQAAAAEGGNcq5ADYNaql3fpAusJjQB115rXsanZTgMYt3uA5t63xY6+1HBb3yFle4iyU3l/w==", "a4009940-9f21-4016-9928-b8fe50341819" });
        }
    }
}
