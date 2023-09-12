using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cell_line_laboratory.Migrations
{
    public partial class UserNStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PlasmidCollection",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "aeeded6f-5789-4c2e-ae65-97be5bd00986", new DateTime(2023, 8, 28, 19, 8, 32, 355, DateTimeKind.Local).AddTicks(7068), "AQAAAAEAACcQAAAAEHNZfAJZbyjM6Ph/x3L4BpheSRN3PQ6gger85o/49ReLc1+jK+QNT1a1DVbS+AjHYQ==", "8652521a-0b99-48a3-88da-3254435405dd" });

            migrationBuilder.CreateIndex(
                name: "IX_PlasmidCollection_UserId",
                table: "PlasmidCollection",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlasmidCollection_User_UserId",
                table: "PlasmidCollection",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlasmidCollection_User_UserId",
                table: "PlasmidCollection");

            migrationBuilder.DropIndex(
                name: "IX_PlasmidCollection_UserId",
                table: "PlasmidCollection");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PlasmidCollection");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Password", "SecurityStamp" },
                values: new object[] { "cd03b2fa-81c8-4c4d-80a4-57a6d95ed4e8", new DateTime(2023, 8, 28, 18, 59, 52, 26, DateTimeKind.Local).AddTicks(3186), "AQAAAAEAACcQAAAAEDxc55O9o3NG+sCOE1pYMrdFbyvRBbbVJJbTwM4okRa0+9Ai0YDZ3YNo9dHQPpusQA==", "d003b3b7-ef39-4aa5-be14-d08c836a0e2a" });
        }
    }
}
