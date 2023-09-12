using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cell_line_laboratory.Migrations
{
    public partial class EquipInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentInventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentInventory", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "07644100-ae52-49ce-aa88-8a6d2d9fcbb9", new DateTime(2023, 9, 5, 19, 40, 28, 263, DateTimeKind.Local).AddTicks(4584), "AQAAAAEAACcQAAAAENTVduhCYNOfMVe+JoWcCoitP++fXTdAPR85LiiTMdkkahqn4SmbSEsZRsUKfs+G+A==", "a2c5bbf7-667f-48bb-a653-ecbb5ef3a7ad" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentInventory");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "34c2ca0c-79cf-4815-8d20-3efd67dad305", new DateTime(2023, 8, 31, 18, 47, 34, 836, DateTimeKind.Local).AddTicks(242), "AQAAAAEAACcQAAAAEKWvskfMDOiw3UlKMQ4/FwVF0CvomT/m8Ys0F/uf7f/Lp3RHmScVXa3zlwOaJKUz+w==", "d0724724-2446-48f1-9377-ad5987838429" });
        }
    }
}
