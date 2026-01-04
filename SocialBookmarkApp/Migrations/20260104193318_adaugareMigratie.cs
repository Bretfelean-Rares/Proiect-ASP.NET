using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialBookmarkApp.Migrations
{
    /// <inheritdoc />
    public partial class adaugareMigratie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Votes_BookmarkId",
                table: "Votes");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_BookmarkId_UserId",
                table: "Votes",
                columns: new[] { "BookmarkId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Votes_BookmarkId_UserId",
                table: "Votes");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_BookmarkId",
                table: "Votes",
                column: "BookmarkId");
        }
    }
}
