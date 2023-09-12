using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cell_line_laboratory.Migrations
{
    public partial class Chemical : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chemical",
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
                    table.PrimaryKey("PK_Chemical", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chemical_User_UserId",
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
                values: new object[] { "3d036735-1e6b-46d3-8872-460447e0fc91", new DateTime(2023, 8, 28, 23, 30, 7, 192, DateTimeKind.Local).AddTicks(8557), "AQAAAAEAACcQAAAAEGK6zwlahzN599nuFh18uLh1TiAIHCjecmqlu0f+Z/lJjKudEw44X9UTO+XOrIU0cw==", "4ba8ba0e-e102-49fc-9a37-329d1cc0ebb2" });

            migrationBuilder.CreateIndex(
                name: "IX_Chemical_UserId",
                table: "Chemical",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chemical");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "c98011b8-56ff-4221-a145-452c0013bef0", new DateTime(2023, 8, 28, 22, 12, 32, 187, DateTimeKind.Local).AddTicks(5539), "AQAAAAEAACcQAAAAEHbuOpexE5oJNjWaHRddShrAwCGhaz98zg/aeFKIt0Z9f6LfUFmOMCd5sEpYyoIyTw==", "1d1c9b0c-b177-46fd-b445-27eddbc89868" });
        }
    }
}
