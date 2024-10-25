using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendGameVibes.Migrations
{
    /// <inheritdoc />
    public partial class changeuserwonerid_forumthread : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumThreads_AspNetUsers_UserId",
                table: "ForumThreads");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ForumThreads",
                newName: "UserOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_ForumThreads_UserId",
                table: "ForumThreads",
                newName: "IX_ForumThreads_UserOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumThreads_AspNetUsers_UserOwnerId",
                table: "ForumThreads",
                column: "UserOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumThreads_AspNetUsers_UserOwnerId",
                table: "ForumThreads");

            migrationBuilder.RenameColumn(
                name: "UserOwnerId",
                table: "ForumThreads",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ForumThreads_UserOwnerId",
                table: "ForumThreads",
                newName: "IX_ForumThreads_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumThreads_AspNetUsers_UserId",
                table: "ForumThreads",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
