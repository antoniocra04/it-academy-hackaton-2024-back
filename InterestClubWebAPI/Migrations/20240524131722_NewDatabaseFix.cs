using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterestClubWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabaseFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubs_Users_Id",
                table: "Clubs");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_Id",
                table: "Events");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorEventId",
                table: "Events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorClubId",
                table: "Clubs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatorEventId",
                table: "Events",
                column: "CreatorEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_CreatorClubId",
                table: "Clubs",
                column: "CreatorClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clubs_Users_CreatorClubId",
                table: "Clubs",
                column: "CreatorClubId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_CreatorEventId",
                table: "Events",
                column: "CreatorEventId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubs_Users_CreatorClubId",
                table: "Clubs");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_CreatorEventId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CreatorEventId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Clubs_CreatorClubId",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "CreatorEventId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CreatorClubId",
                table: "Clubs");

            migrationBuilder.AddForeignKey(
                name: "FK_Clubs_Users_Id",
                table: "Clubs",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_Id",
                table: "Events",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
