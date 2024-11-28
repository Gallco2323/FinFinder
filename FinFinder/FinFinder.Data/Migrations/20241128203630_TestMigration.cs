using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinFinder.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FishingTechniques",
                keyColumn: "Id",
                keyValue: new Guid("0aacc3fb-59c4-414b-a46f-d1e23ef639f2"));

            migrationBuilder.DeleteData(
                table: "FishingTechniques",
                keyColumn: "Id",
                keyValue: new Guid("107799da-1d49-4f8b-b584-f4a5c6c9bd10"));

            migrationBuilder.DeleteData(
                table: "FishingTechniques",
                keyColumn: "Id",
                keyValue: new Guid("3e526001-81fe-4a9a-87b7-7fdb3df52fd5"));

            migrationBuilder.DeleteData(
                table: "FishingTechniques",
                keyColumn: "Id",
                keyValue: new Guid("7f2074b5-5169-4ad7-bbad-e7e722306535"));

            migrationBuilder.InsertData(
                table: "FishingTechniques",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("c4b55e4e-a700-4bb6-917a-247f07d51c25"), "Trolling" },
                    { new Guid("d6a43b7f-8eca-4373-924b-ce078d301304"), "Fly Fishing" },
                    { new Guid("ea2fc665-8c68-4041-96bb-118f0f552061"), "Bait Fishing" },
                    { new Guid("edab01bb-fdf3-4c58-98d8-3f6d9fc0fedb"), "Spin Fishing" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FishingTechniques",
                keyColumn: "Id",
                keyValue: new Guid("c4b55e4e-a700-4bb6-917a-247f07d51c25"));

            migrationBuilder.DeleteData(
                table: "FishingTechniques",
                keyColumn: "Id",
                keyValue: new Guid("d6a43b7f-8eca-4373-924b-ce078d301304"));

            migrationBuilder.DeleteData(
                table: "FishingTechniques",
                keyColumn: "Id",
                keyValue: new Guid("ea2fc665-8c68-4041-96bb-118f0f552061"));

            migrationBuilder.DeleteData(
                table: "FishingTechniques",
                keyColumn: "Id",
                keyValue: new Guid("edab01bb-fdf3-4c58-98d8-3f6d9fc0fedb"));

            migrationBuilder.InsertData(
                table: "FishingTechniques",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0aacc3fb-59c4-414b-a46f-d1e23ef639f2"), "Bait Fishing" },
                    { new Guid("107799da-1d49-4f8b-b584-f4a5c6c9bd10"), "Fly Fishing" },
                    { new Guid("3e526001-81fe-4a9a-87b7-7fdb3df52fd5"), "Spin Fishing" },
                    { new Guid("7f2074b5-5169-4ad7-bbad-e7e722306535"), "Trolling" }
                });
        }
    }
}
