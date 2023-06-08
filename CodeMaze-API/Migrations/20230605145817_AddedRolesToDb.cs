using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CodeMaze_API.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7160b87d-74d9-4354-97f2-1ea058b10f5b", "dbb12ebb-e203-4252-82fa-5326e82f735c", "Administrator", "ADMINISTRATOR" },
                    { "f79d100f-e36f-4858-bb21-882855c99eb7", "da56494d-e6de-475d-af8a-3f77aff51fd8", "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7160b87d-74d9-4354-97f2-1ea058b10f5b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f79d100f-e36f-4858-bb21-882855c99eb7");
        }
    }
}
