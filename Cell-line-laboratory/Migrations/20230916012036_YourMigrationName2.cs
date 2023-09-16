using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cell_line_laboratory.Migrations
{
    public partial class YourMigrationName2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableProducts",
                table: "EquipmentInventoryModel");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "EquipmentInventory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "EquipmentInventory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "EquipmentInventory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "47291f15-ff7a-4855-b2fe-6d63f4a2d75f", new DateTime(2023, 9, 16, 2, 20, 36, 461, DateTimeKind.Local).AddTicks(5469), "AQAAAAEAACcQAAAAEFp7D/YRzrRzzeeKTfipRgvfYyprIpvRjXU/DfHczRCd77631/DnyWqtXni9f3tq7Q==", "ee2239d3-e52f-46e1-988a-b0a92fb240f1" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvailableProducts",
                table: "EquipmentInventoryModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "EquipmentInventory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "EquipmentInventory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "EquipmentInventory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "131b24b4-6e09-47c1-b5f6-06c788522104", new DateTime(2023, 9, 15, 22, 57, 53, 952, DateTimeKind.Local).AddTicks(1186), "AQAAAAEAACcQAAAAEN6gTO9gyOfUeyRVQx5exjjUIS98L+wbP449HfbmpslSjbKhYjDSXGckN92frkuA0w==", "53c4036e-31cb-47ec-853c-60c8750ae55d" });
        }
    }
}
