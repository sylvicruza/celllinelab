using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cell_line_laboratory.Migrations
{
    public partial class AssignTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "AssignTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignTask_User_UserId",
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
                values: new object[] { "34c2ca0c-79cf-4815-8d20-3efd67dad305", new DateTime(2023, 8, 31, 18, 47, 34, 836, DateTimeKind.Local).AddTicks(242), "AQAAAAEAACcQAAAAEKWvskfMDOiw3UlKMQ4/FwVF0CvomT/m8Ys0F/uf7f/Lp3RHmScVXa3zlwOaJKUz+w==", "d0724724-2446-48f1-9377-ad5987838429" });

            migrationBuilder.CreateIndex(
                name: "IX_AssignTask_UserId",
                table: "AssignTask",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignTask");

         

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "3d036735-1e6b-46d3-8872-460447e0fc91", new DateTime(2023, 8, 28, 23, 30, 7, 192, DateTimeKind.Local).AddTicks(8557), "AQAAAAEAACcQAAAAEGK6zwlahzN599nuFh18uLh1TiAIHCjecmqlu0f+Z/lJjKudEw44X9UTO+XOrIU0cw==", "4ba8ba0e-e102-49fc-9a37-329d1cc0ebb2" });
        }
    }
}
