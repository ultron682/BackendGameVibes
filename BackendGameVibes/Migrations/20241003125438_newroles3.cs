using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendGameVibes.Migrations
{
    /// <inheritdoc />
    public partial class newroles3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_IdentityRoleId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IdentityRoleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdentityRoleId",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityRoleId",
                table: "AspNetUsers",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdentityRoleId",
                table: "AspNetUsers",
                column: "IdentityRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_IdentityRoleId",
                table: "AspNetUsers",
                column: "IdentityRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");
        }
    }
}
