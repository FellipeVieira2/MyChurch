using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesToFamilyAndGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "family_id",
                schema: "postgres",
                table: "member",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "families",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "integer", nullable: false),
                    family_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_families", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "family_invitations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "integer", nullable: false),
                    inviter_member_id = table.Column<int>(type: "integer", nullable: false),
                    InvitedMemberId = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    responded_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_family_invitations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "group",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    leader_id = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    accepts_new_members = table.Column<bool>(type: "boolean", nullable: false),
                    cover_image_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group", x => x.id);
                    table.ForeignKey(
                        name: "FK_group_member_leader_id",
                        column: x => x.leader_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "group_resource",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    file_url = table.Column<string>(type: "text", nullable: false),
                    uploaded_by_member_id = table.Column<int>(type: "integer", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_resource", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "child",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    family_id = table.Column<int>(type: "integer", nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_child", x => x.id);
                    table.ForeignKey(
                        name: "FK_child_families_family_id",
                        column: x => x.family_id,
                        principalTable: "families",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_member",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    role_in_group = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    date_joined = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_member", x => x.id);
                    table.ForeignKey(
                        name: "FK_group_member_group_group_id",
                        column: x => x.group_id,
                        principalTable: "group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_member_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "child_group_assignments",
                columns: table => new
                {
                    child_id = table.Column<int>(type: "integer", nullable: false),
                    group_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_child_group_assignments", x => new { x.child_id, x.group_id });
                    table.ForeignKey(
                        name: "FK_child_group_assignments_child_child_id",
                        column: x => x.child_id,
                        principalTable: "child",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_child_group_assignments_group_group_id",
                        column: x => x.group_id,
                        principalTable: "group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_member_family_id",
                schema: "postgres",
                table: "member",
                column: "family_id");

            migrationBuilder.CreateIndex(
                name: "IX_child_family_id",
                table: "child",
                column: "family_id");

            migrationBuilder.CreateIndex(
                name: "IX_child_group_assignments_group_id",
                table: "child_group_assignments",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_group_leader_id",
                table: "group",
                column: "leader_id");

            migrationBuilder.CreateIndex(
                name: "IX_group_member_group_id_member_id",
                table: "group_member",
                columns: new[] { "group_id", "member_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_member_member_id",
                table: "group_member",
                column: "member_id");

            migrationBuilder.AddForeignKey(
                name: "FK_member_families_family_id",
                schema: "postgres",
                table: "member",
                column: "family_id",
                principalTable: "families",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_member_families_family_id",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropTable(
                name: "child_group_assignments");

            migrationBuilder.DropTable(
                name: "family_invitations");

            migrationBuilder.DropTable(
                name: "group_member");

            migrationBuilder.DropTable(
                name: "group_resource");

            migrationBuilder.DropTable(
                name: "child");

            migrationBuilder.DropTable(
                name: "group");

            migrationBuilder.DropTable(
                name: "families");

            migrationBuilder.DropIndex(
                name: "IX_member_family_id",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropColumn(
                name: "family_id",
                schema: "postgres",
                table: "member");
        }
    }
}
