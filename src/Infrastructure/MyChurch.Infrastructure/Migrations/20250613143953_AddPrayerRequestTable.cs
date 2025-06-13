using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrayerRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "prayer_request",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worship_service_id = table.Column<int>(type: "integer", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    request = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prayer_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_prayer_request_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_prayer_request_worship_services_worship_service_id",
                        column: x => x.worship_service_id,
                        principalSchema: "postgres",
                        principalTable: "worship_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prayer_request_member_id",
                schema: "postgres",
                table: "prayer_request",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_prayer_request_worship_service_id",
                schema: "postgres",
                table: "prayer_request",
                column: "worship_service_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prayer_request",
                schema: "postgres");
        }
    }
}
