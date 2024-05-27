using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterestClubWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class descriptionFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullDescription",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Discussions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullDescription",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Discussions");
        }
    }
}
