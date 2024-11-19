using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinFinder.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Observations");

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

            migrationBuilder.DropColumn(
                name: "PhotoURL",
                table: "FishCatches");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "FishCatches",
                newName: "LocationName");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FishCatches",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "FishCatches",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "FishCatches",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FishCatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LikedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_FishCatches_FishCatchId",
                        column: x => x.FishCatchId,
                        principalTable: "FishCatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FishCatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_FishCatches_FishCatchId",
                        column: x => x.FishCatchId,
                        principalTable: "FishCatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Likes_FishCatchId",
                table: "Likes",
                column: "FishCatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_FishCatchId",
                table: "Photos",
                column: "FishCatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Photos");

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

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FishCatches");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "FishCatches");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "FishCatches");

            migrationBuilder.RenameColumn(
                name: "LocationName",
                table: "FishCatches",
                newName: "Location");

            migrationBuilder.AddColumn<string>(
                name: "PhotoURL",
                table: "FishCatches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FishCatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Bait = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Observations_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Observations_FishCatches_FishCatchId",
                        column: x => x.FishCatchId,
                        principalTable: "FishCatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Observations_ApplicationUserId",
                table: "Observations",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_FishCatchId",
                table: "Observations",
                column: "FishCatchId",
                unique: true);
        }
    }
}
