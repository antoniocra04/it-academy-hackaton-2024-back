using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterestClubWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class FullDiscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullDescription",
                table: "Clubs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullDescription",
                table: "Clubs");
        }
    }
}
