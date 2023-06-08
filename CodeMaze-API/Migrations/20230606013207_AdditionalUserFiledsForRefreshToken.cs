using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CodeMaze_API.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalUserFiledsForRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7160b87d-74d9-4354-97f2-1ea058b10f5b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f79d100f-e36f-4858-bb21-882855c99eb7");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0fd9ca6c-8ddf-416d-a48e-ef2b4fde7852", "a9bb9ee0-e812-4bf6-932a-8f6080ece9de", "Administrator", "ADMINISTRATOR" },
                    { "b09d115c-eb63-4b09-b4a3-1f19548de9ed", "57cd9a04-f19b-41e8-a0cb-97c61cdd4dc7", "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0fd9ca6c-8ddf-416d-a48e-ef2b4fde7852");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b09d115c-eb63-4b09-b4a3-1f19548de9ed");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7160b87d-74d9-4354-97f2-1ea058b10f5b", "dbb12ebb-e203-4252-82fa-5326e82f735c", "Administrator", "ADMINISTRATOR" },
                    { "f79d100f-e36f-4858-bb21-882855c99eb7", "da56494d-e6de-475d-af8a-3f77aff51fd8", "Manager", "MANAGER" }
                });
        }
    }
}
