using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendGameVibes.Migrations
{
    /// <inheritdoc />
    public partial class newheaderimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeaderImage",
                table: "Games",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeaderImage",
                table: "Games");
        }
    }
}
