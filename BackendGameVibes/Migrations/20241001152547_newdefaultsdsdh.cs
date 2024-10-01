using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendGameVibes.Migrations
{
    /// <inheritdoc />
    public partial class newdefaultsdsdh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SteamId",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SteamId",
                table: "Games");
        }
    }
}
