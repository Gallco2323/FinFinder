using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinFinder.Data.Migrations
{
    /// <inheritdoc />
    public partial class PopulatingFishingTechnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FishingTechniques",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("748dfa61-51d2-4064-b6a4-35aace504cef"), "Spin Fishing" },
                    { new Guid("85939209-8576-4811-bf8c-79a2f888a284"), "Fly Fishing" },
                    { new Guid("a41c63a4-14ab-4321-a994-e6f4eefeb9bc"), "Trolling" },
                    { new Guid("b1e006fa-9d41-4375-a895-652b59f1abf5"), "Bait Fishing" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FishingTechniques",
                keyColumn: "Id",
                keyValue: new Guid("748dfa61-51d2-4064-b6a4-35aace504cef"));

            migrationBuilder.DeleteData(
                table: "FishingTechniques",
                keyColumn: "Id",
                keyValue: new Guid("85939209-8576-4811-bf8c-79a2f888a284"));

            migrationBuilder.DeleteData(
                table: "FishingTechniques",
                keyColumn: "Id",
                keyValue: new Guid("a41c63a4-14ab-4321-a994-e6f4eefeb9bc"));

            migrationBuilder.DeleteData(
                table: "FishingTechniques",
                keyColumn: "Id",
                keyValue: new Guid("b1e006fa-9d41-4375-a895-652b59f1abf5"));
        }
    }
}
