using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTablesToBiblePlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bible_reading_plan",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(100)", nullable: false),
                    description = table.Column<string>(type: "varchar(500)", nullable: false),
                    duration_in_days = table.Column<int>(type: "int", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bible_reading_plan", x => x.id);
                    table.ForeignKey(
                        name: "FK_bible_reading_plan_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bible_reading_plan_stage",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bible_reading_plan_id = table.Column<int>(type: "int", nullable: false),
                    order = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "varchar(200)", nullable: false),
                    verse_references = table.Column<string>(type: "varchar(500)", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bible_reading_plan_stage", x => x.id);
                    table.ForeignKey(
                        name: "FK_bible_reading_plan_stage_bible_reading_plan_bible_reading_p~",
                        column: x => x.bible_reading_plan_id,
                        principalSchema: "postgres",
                        principalTable: "bible_reading_plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "member_bible_reading_progress",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    bible_reading_plan_id = table.Column<int>(type: "int", nullable: false),
                    bible_reading_plan_stage_id = table.Column<int>(type: "int", nullable: false),
                    date_completed = table.Column<DateTime>(type: "timestamp", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_bible_reading_progress", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_bible_reading_progress_bible_reading_plan_bible_read~",
                        column: x => x.bible_reading_plan_id,
                        principalSchema: "postgres",
                        principalTable: "bible_reading_plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_member_bible_reading_progress_bible_reading_plan_stage_bibl~",
                        column: x => x.bible_reading_plan_stage_id,
                        principalSchema: "postgres",
                        principalTable: "bible_reading_plan_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_member_bible_reading_progress_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bible_reading_plan_church_id",
                schema: "postgres",
                table: "bible_reading_plan",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_bible_reading_plan_stage_bible_reading_plan_id_order",
                schema: "postgres",
                table: "bible_reading_plan_stage",
                columns: new[] { "bible_reading_plan_id", "order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_member_bible_reading_progress_bible_reading_plan_id",
                schema: "postgres",
                table: "member_bible_reading_progress",
                column: "bible_reading_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_bible_reading_progress_bible_reading_plan_stage_id",
                schema: "postgres",
                table: "member_bible_reading_progress",
                column: "bible_reading_plan_stage_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_bible_reading_progress_member_id_bible_reading_plan_~",
                schema: "postgres",
                table: "member_bible_reading_progress",
                columns: new[] { "member_id", "bible_reading_plan_stage_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "member_bible_reading_progress",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "bible_reading_plan_stage",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "bible_reading_plan",
                schema: "postgres");
        }
    }
}
