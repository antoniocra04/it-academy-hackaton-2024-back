using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterestClubWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabaseFix3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubs_Users_CreatorClubId",
                table: "Clubs");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Clubs_ClubId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_CreatorEventId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CreatorEventId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Clubs_CreatorClubId",
                table: "Clubs");

            migrationBuilder.RenameColumn(
                name: "CreatorEventId",
                table: "Events",
                newName: "CreatorEventID");

            migrationBuilder.RenameColumn(
                name: "ClubId",
                table: "Events",
                newName: "ClubID");

            migrationBuilder.RenameIndex(
                name: "IX_Events_ClubId",
                table: "Events",
                newName: "IX_Events_ClubID");

            migrationBuilder.RenameColumn(
                name: "CreatorClubId",
                table: "Clubs",
                newName: "CreatorClubID");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Clubs_ClubID",
                table: "Events",
                column: "ClubID",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Clubs_ClubID",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "CreatorEventID",
                table: "Events",
                newName: "CreatorEventId");

            migrationBuilder.RenameColumn(
                name: "ClubID",
                table: "Events",
                newName: "ClubId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_ClubID",
                table: "Events",
                newName: "IX_Events_ClubId");

            migrationBuilder.RenameColumn(
                name: "CreatorClubID",
                table: "Clubs",
                newName: "CreatorClubId");

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
                name: "FK_Events_Clubs_ClubId",
                table: "Events",
                column: "ClubId",
                principalTable: "Clubs",
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
    }
}
