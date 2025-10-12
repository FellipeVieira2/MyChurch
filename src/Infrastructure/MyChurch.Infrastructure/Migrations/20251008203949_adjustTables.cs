using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adjustTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "member_custom_permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    permission = table.Column<int>(type: "integer", nullable: false),
                    is_granted = table.Column<bool>(type: "boolean", nullable: false),
                    granted_by_member_id = table.Column<int>(type: "int", nullable: true),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_custom_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_custom_permissions_member_granted_by_member_id",
                        column: x => x.granted_by_member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_member_custom_permissions_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role = table.Column<int>(type: "integer", nullable: false),
                    permission = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    church_id = table.Column<int>(type: "int", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_permissions_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_member_custom_permissions_expires_at",
                table: "member_custom_permissions",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_member_custom_permissions_granted_by_member_id",
                table: "member_custom_permissions",
                column: "granted_by_member_id");

            migrationBuilder.CreateIndex(
                name: "ix_member_custom_permissions_member_id",
                table: "member_custom_permissions",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "ix_member_custom_permissions_member_permission",
                table: "member_custom_permissions",
                columns: new[] { "member_id", "permission" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_church_id",
                table: "role_permissions",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_role_permission_church",
                table: "role_permissions",
                columns: new[] { "role", "permission", "church_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "member_custom_permissions");

            migrationBuilder.DropTable(
                name: "role_permissions");
        }
    }
}
