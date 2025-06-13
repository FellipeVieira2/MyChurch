using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesToWorshipControll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventType",
                schema: "postgres",
                table: "event",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorshipServiceId",
                schema: "postgres",
                table: "event",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "worship_services",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    theme = table.Column<string>(type: "text", nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worship_services", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "worship_activities",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worship_service_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    order = table.Column<int>(type: "integer", nullable: false),
                    is_current = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worship_activities", x => x.id);
                    table.ForeignKey(
                        name: "FK_worship_activities_worship_services_worship_service_id",
                        column: x => x.worship_service_id,
                        principalSchema: "postgres",
                        principalTable: "worship_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worship_presences",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worship_service_id = table.Column<int>(type: "integer", nullable: false),
                    member_id = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worship_presences", x => x.id);
                    table.ForeignKey(
                        name: "FK_worship_presences_worship_services_worship_service_id",
                        column: x => x.worship_service_id,
                        principalSchema: "postgres",
                        principalTable: "worship_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worship_activity_bibles",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worship_activity_id = table.Column<int>(type: "integer", nullable: false),
                    bible_version_id = table.Column<int>(type: "integer", nullable: false),
                    book_id = table.Column<int>(type: "integer", nullable: false),
                    chapter_id = table.Column<int>(type: "integer", nullable: false),
                    verse_start = table.Column<int>(type: "integer", nullable: false),
                    verse_end = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worship_activity_bibles", x => x.id);
                    table.ForeignKey(
                        name: "FK_worship_activity_bibles_worship_activities_worship_activity~",
                        column: x => x.worship_activity_id,
                        principalSchema: "postgres",
                        principalTable: "worship_activities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worship_activity_hymns",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worship_activity_id = table.Column<int>(type: "integer", nullable: false),
                    hymn_id = table.Column<int>(type: "integer", nullable: false),
                    hymn_title = table.Column<string>(type: "text", nullable: true),
                    hymn_number = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worship_activity_hymns", x => x.id);
                    table.ForeignKey(
                        name: "FK_worship_activity_hymns_worship_activities_worship_activity_~",
                        column: x => x.worship_activity_id,
                        principalSchema: "postgres",
                        principalTable: "worship_activities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_WorshipServiceId",
                schema: "postgres",
                table: "event",
                column: "WorshipServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_worship_activities_worship_service_id",
                schema: "postgres",
                table: "worship_activities",
                column: "worship_service_id");

            migrationBuilder.CreateIndex(
                name: "IX_worship_activity_bibles_worship_activity_id",
                schema: "postgres",
                table: "worship_activity_bibles",
                column: "worship_activity_id");

            migrationBuilder.CreateIndex(
                name: "IX_worship_activity_hymns_worship_activity_id",
                schema: "postgres",
                table: "worship_activity_hymns",
                column: "worship_activity_id");

            migrationBuilder.CreateIndex(
                name: "IX_worship_presences_worship_service_id",
                schema: "postgres",
                table: "worship_presences",
                column: "worship_service_id");

            migrationBuilder.AddForeignKey(
                name: "FK_event_worship_services_WorshipServiceId",
                schema: "postgres",
                table: "event",
                column: "WorshipServiceId",
                principalSchema: "postgres",
                principalTable: "worship_services",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_worship_services_WorshipServiceId",
                schema: "postgres",
                table: "event");

            migrationBuilder.DropTable(
                name: "worship_activity_bibles",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "worship_activity_hymns",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "worship_presences",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "worship_activities",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "worship_services",
                schema: "postgres");

            migrationBuilder.DropIndex(
                name: "IX_event_WorshipServiceId",
                schema: "postgres",
                table: "event");

            migrationBuilder.DropColumn(
                name: "EventType",
                schema: "postgres",
                table: "event");

            migrationBuilder.DropColumn(
                name: "WorshipServiceId",
                schema: "postgres",
                table: "event");
        }
    }
}
