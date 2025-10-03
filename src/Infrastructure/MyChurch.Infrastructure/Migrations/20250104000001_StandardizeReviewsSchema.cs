using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StandardizeReviewsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Move ReviewVotes table to postgres schema
            migrationBuilder.RenameTable(
                name: "ReviewVotes",
                newName: "review_votes",
                newSchema: "postgres");

            // Move Reviews table to postgres schema
            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "reviews",
                newSchema: "postgres");

            // Move ReviewPhotos table to postgres schema
            migrationBuilder.RenameTable(
                name: "ReviewPhotos",
                newName: "review_photos",
                newSchema: "postgres");

            // Move ReviewResponses table to postgres schema
            migrationBuilder.RenameTable(
                name: "ReviewResponses",
                newName: "review_responses",
                newSchema: "postgres");

            // Rename columns to snake_case for consistency
            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "postgres",
                table: "review_votes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ReviewId",
                schema: "postgres",
                table: "review_votes",
                newName: "review_id");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                schema: "postgres",
                table: "review_votes",
                newName: "member_id");

            migrationBuilder.RenameColumn(
                name: "IsHelpful",
                schema: "postgres",
                table: "review_votes",
                newName: "is_helpful");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "postgres",
                table: "review_votes",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "postgres",
                table: "review_votes",
                newName: "updated_at");

            // Update indexes
            migrationBuilder.RenameIndex(
                name: "IX_ReviewVotes_MemberId",
                schema: "postgres",
                table: "review_votes",
                newName: "ix_review_votes_member_id");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewVotes_ReviewId",
                schema: "postgres",
                table: "review_votes",
                newName: "ix_review_votes_review_id");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewVotes_ReviewId_MemberId",
                schema: "postgres",
                table: "review_votes",
                newName: "ix_review_votes_review_id_member_id");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewVotes_ReviewId_IsHelpful",
                schema: "postgres",
                table: "review_votes",
                newName: "ix_review_votes_review_id_is_helpful");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert schema changes
            migrationBuilder.RenameTable(
                name: "review_votes",
                schema: "postgres",
                newName: "ReviewVotes");

            migrationBuilder.RenameTable(
                name: "reviews",
                schema: "postgres",
                newName: "Reviews");

            migrationBuilder.RenameTable(
                name: "review_photos",
                schema: "postgres",
                newName: "ReviewPhotos");

            migrationBuilder.RenameTable(
                name: "review_responses",
                schema: "postgres",
                newName: "ReviewResponses");

            // Revert column names
            migrationBuilder.RenameColumn(
                name: "id",
                table: "ReviewVotes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "review_id",
                table: "ReviewVotes",
                newName: "ReviewId");

            migrationBuilder.RenameColumn(
                name: "member_id",
                table: "ReviewVotes",
                newName: "MemberId");

            migrationBuilder.RenameColumn(
                name: "is_helpful",
                table: "ReviewVotes",
                newName: "IsHelpful");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ReviewVotes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ReviewVotes",
                newName: "UpdatedAt");

            // Revert indexes
            migrationBuilder.RenameIndex(
                name: "ix_review_votes_member_id",
                table: "ReviewVotes",
                newName: "IX_ReviewVotes_MemberId");

            migrationBuilder.RenameIndex(
                name: "ix_review_votes_review_id",
                table: "ReviewVotes",
                newName: "IX_ReviewVotes_ReviewId");

            migrationBuilder.RenameIndex(
                name: "ix_review_votes_review_id_member_id",
                table: "ReviewVotes",
                newName: "IX_ReviewVotes_ReviewId_MemberId");

            migrationBuilder.RenameIndex(
                name: "ix_review_votes_review_id_is_helpful",
                table: "ReviewVotes",
                newName: "IX_ReviewVotes_ReviewId_IsHelpful");
        }
    }
}
