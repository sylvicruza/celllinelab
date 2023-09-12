using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cell_line_laboratory.Migrations
{
    public partial class EnzymesNChemicalNAntibody : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Enzyme",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlasmidCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AntibodyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CatalogueNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Maker = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMarkedForDeletion = table.Column<bool>(type: "bit", nullable: false),
                    DeletionTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enzyme", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enzyme_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "c98011b8-56ff-4221-a145-452c0013bef0", new DateTime(2023, 8, 28, 22, 12, 32, 187, DateTimeKind.Local).AddTicks(5539), "AQAAAAEAACcQAAAAEHbuOpexE5oJNjWaHRddShrAwCGhaz98zg/aeFKIt0Z9f6LfUFmOMCd5sEpYyoIyTw==", "1d1c9b0c-b177-46fd-b445-27eddbc89868" });

            migrationBuilder.CreateIndex(
                name: "IX_Enzyme_UserId",
                table: "Enzyme",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enzyme");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "aeeded6f-5789-4c2e-ae65-97be5bd00986", new DateTime(2023, 8, 28, 19, 8, 32, 355, DateTimeKind.Local).AddTicks(7068), "AQAAAAEAACcQAAAAEHNZfAJZbyjM6Ph/x3L4BpheSRN3PQ6gger85o/49ReLc1+jK+QNT1a1DVbS+AjHYQ==", "8652521a-0b99-48a3-88da-3254435405dd" });
        }
    }
}
