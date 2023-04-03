using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HallHaven.Migrations
{
    public partial class testAddUserRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HallHavenUserId",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AspNetUserId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_HallHavenUserId",
                table: "Users",
                column: "HallHavenUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AspNetUsers_HallHavenUserId",
                table: "Users",
                column: "HallHavenUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_AspNetUsers_HallHavenUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_HallHavenUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HallHavenUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AspNetUserId",
                table: "AspNetUsers");
        }
    }
}
