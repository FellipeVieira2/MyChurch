using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentGeneralLeaderScopesAndFeedVisibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "visible_to_branches",
                schema: "postgres",
                table: "feed_posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "department_general_leader_scopes",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parent_church_id = table.Column<int>(type: "int", nullable: false),
                    department_id = table.Column<int>(type: "integer", nullable: false),
                    leader_member_id = table.Column<int>(type: "int", nullable: false),
                    branch_church_id = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department_general_leader_scopes", x => x.id);
                    table.ForeignKey(
                        name: "FK_department_general_leader_scopes_church_branch_church_id",
                        column: x => x.branch_church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_department_general_leader_scopes_church_parent_church_id",
                        column: x => x.parent_church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_department_general_leader_scopes_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "postgres",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_department_general_leader_scopes_member_leader_member_id",
                        column: x => x.leader_member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_department_general_leader_scopes_branch_church_id",
                schema: "postgres",
                table: "department_general_leader_scopes",
                column: "branch_church_id");

            migrationBuilder.CreateIndex(
                name: "IX_department_general_leader_scopes_department_id",
                schema: "postgres",
                table: "department_general_leader_scopes",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_department_general_leader_scopes_leader_member_id",
                schema: "postgres",
                table: "department_general_leader_scopes",
                column: "leader_member_id");

            migrationBuilder.CreateIndex(
                name: "IX_department_general_leader_scopes_parent_church_id_departmen~",
                schema: "postgres",
                table: "department_general_leader_scopes",
                columns: new[] { "parent_church_id", "department_id", "leader_member_id", "branch_church_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "department_general_leader_scopes",
                schema: "postgres");

            migrationBuilder.DropColumn(
                name: "visible_to_branches",
                schema: "postgres",
                table: "feed_posts");
        }
    }
}
