using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "member_configurations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    preferred_bible_version_id = table.Column<int>(type: "integer", nullable: true),
                    theme_preference = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Light"),
                    font_size = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Medium"),
                    enable_notifications = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    last_updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_configurations", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_configurations_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_member_configurations_versions_preferred_bible_version_id",
                        column: x => x.preferred_bible_version_id,
                        principalSchema: "postgres",
                        principalTable: "versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_member_configurations_member_id_unique",
                table: "member_configurations",
                column: "member_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_member_configurations_preferred_bible_version_id",
                table: "member_configurations",
                column: "preferred_bible_version_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "member_configurations");
        }
    }
}
