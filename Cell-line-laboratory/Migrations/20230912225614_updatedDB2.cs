using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cell_line_laboratory.Migrations
{
    public partial class updatedDB2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "EquipmentInventory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "1cf3787e-88af-428d-b142-fdf1ccf5754a", new DateTime(2023, 9, 12, 23, 56, 13, 773, DateTimeKind.Local).AddTicks(356), "AQAAAAEAACcQAAAAEGGNcq5ADYNaql3fpAusJjQB115rXsanZTgMYt3uA5t63xY6+1HBb3yFle4iyU3l/w==", "a4009940-9f21-4016-9928-b8fe50341819" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "EquipmentInventory",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "56ccd3d3-3d6a-4f0d-8cb8-dc02077b55be", new DateTime(2023, 9, 12, 21, 20, 41, 260, DateTimeKind.Local).AddTicks(4876), "AQAAAAEAACcQAAAAEMqUWfHhMzTCi7mQB7SgrJ4Q5u8okjmzMfWigl7XGs2KCf4n3o7VS5fiXM3uFzpuhQ==", "3f8be96b-4f53-4556-97a1-c0a91395c927" });
        }
    }
}
