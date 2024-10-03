using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackendGameVibes.Migrations
{
    /// <inheritdoc />
    public partial class newroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "39adab77-4108-448e-bb00-0aba177a1fa5", null, "user", null },
                    { "4dfde565-9bab-4697-b414-0a9d0e93cdfa", null, "mod", null },
                    { "b7de4fa5-a145-457d-9079-5e5e14d2bacb", null, "guest", null },
                    { "b9cfbff1-2b74-4096-b9e2-9fdd1abba52d", null, "admin", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39adab77-4108-448e-bb00-0aba177a1fa5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4dfde565-9bab-4697-b414-0a9d0e93cdfa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b7de4fa5-a145-457d-9079-5e5e14d2bacb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b9cfbff1-2b74-4096-b9e2-9fdd1abba52d");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: null,
                column: "Name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "guest" },
                    { 2, "user" },
                    { 3, "mod" },
                    { 4, "admin" }
                });
        }
    }
}
