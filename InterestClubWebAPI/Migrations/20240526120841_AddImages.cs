using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterestClubWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EventImageId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClubImageId",
                table: "Clubs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageName = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventImageId",
                table: "Events",
                column: "EventImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_ClubImageId",
                table: "Clubs",
                column: "ClubImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clubs_Images_ClubImageId",
                table: "Clubs",
                column: "ClubImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Images_EventImageId",
                table: "Events",
                column: "EventImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubs_Images_ClubImageId",
                table: "Clubs");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Images_EventImageId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventImageId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Clubs_ClubImageId",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "EventImageId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ClubImageId",
                table: "Clubs");
        }
    }
}
