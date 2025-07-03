using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberBibleReadingAssignmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "member_bible_reading_assignment",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    bible_reading_plan_id = table.Column<int>(type: "int", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_bible_reading_assignment", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_member_bible_reading_assignment_member_id_bible_reading_pla~",
                schema: "postgres",
                table: "member_bible_reading_assignment",
                columns: new[] { "member_id", "bible_reading_plan_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "member_bible_reading_assignment",
                schema: "postgres");
        }
    }
}
