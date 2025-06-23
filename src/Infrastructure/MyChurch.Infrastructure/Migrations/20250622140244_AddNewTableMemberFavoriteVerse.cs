using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTableMemberFavoriteVerse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "member_journey_progresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresLeaderVerification",
                table: "journey_stages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "member_favorite_verses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    version_id = table.Column<int>(type: "integer", nullable: false),
                    book_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    chapter_number = table.Column<int>(type: "integer", nullable: false),
                    verse_number = table.Column<int>(type: "integer", nullable: false),
                    date_favorited = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_favorite_verses", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_favorite_verses_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_member_journey_progresses_journey_stage_id",
                table: "member_journey_progresses",
                column: "journey_stage_id");

            migrationBuilder.CreateIndex(
                name: "ix_member_favorite_verses_unique_verse",
                table: "member_favorite_verses",
                columns: new[] { "member_id", "version_id", "book_name", "chapter_number", "verse_number" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_member_journey_progresses_journey_stages_journey_stage_id",
                table: "member_journey_progresses",
                column: "journey_stage_id",
                principalTable: "journey_stages",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_member_journey_progresses_journey_stages_journey_stage_id",
                table: "member_journey_progresses");

            migrationBuilder.DropTable(
                name: "member_favorite_verses");

            migrationBuilder.DropIndex(
                name: "IX_member_journey_progresses_journey_stage_id",
                table: "member_journey_progresses");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "member_journey_progresses");

            migrationBuilder.DropColumn(
                name: "RequiresLeaderVerification",
                table: "journey_stages");
        }
    }
}
