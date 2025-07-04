using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPresentationAndSlideModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "presentations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    admin_user_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    current_slide_index = table.Column<int>(type: "integer", nullable: false),
                    is_live = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_presentations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_presentations_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_presentations_member_admin_user_id",
                        column: x => x.admin_user_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "slides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    presentation_id = table.Column<int>(type: "integer", nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    content_type = table.Column<int>(type: "integer", nullable: false),
                    content_reference_json = table.Column<string>(type: "jsonb", nullable: false),
                    cached_display_text = table.Column<string>(type: "text", nullable: false),
                    cached_media_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_slides_presentations_presentation_id",
                        column: x => x.presentation_id,
                        principalTable: "presentations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_presentations_admin_user_id",
                table: "presentations",
                column: "admin_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_presentations_church_id",
                table: "presentations",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_slides_presentation_id",
                table: "slides",
                column: "presentation_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "slides");

            migrationBuilder.DropTable(
                name: "presentations");
        }
    }
}
