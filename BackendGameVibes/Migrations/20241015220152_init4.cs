using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendGameVibes.Migrations
{
    /// <inheritdoc />
    public partial class init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_UserGameVibesId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserGameVibesId1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UserGameVibesId1",
                table: "Reviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserGameVibesId1",
                table: "Reviews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserGameVibesId1",
                table: "Reviews",
                column: "UserGameVibesId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_UserGameVibesId1",
                table: "Reviews",
                column: "UserGameVibesId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
