using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendGameVibes.Migrations
{
    /// <inheritdoc />
    public partial class changegameReleaseDatefromstringtoDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersGamesFollow",
                columns: table => new
                {
                    FollowedGamesId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayersFollowingId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersGamesFollow", x => new { x.FollowedGamesId, x.PlayersFollowingId });
                    table.ForeignKey(
                        name: "FK_UsersGamesFollow_AspNetUsers_PlayersFollowingId",
                        column: x => x.PlayersFollowingId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersGamesFollow_Games_FollowedGamesId",
                        column: x => x.FollowedGamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersGamesFollow_PlayersFollowingId",
                table: "UsersGamesFollow",
                column: "PlayersFollowingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersGamesFollow");
        }
    }
}
