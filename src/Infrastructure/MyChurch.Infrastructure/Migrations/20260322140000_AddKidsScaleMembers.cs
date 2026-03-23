using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    public partial class AddKidsScaleMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "kids_scale_members",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    role_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kids_scale_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_kids_scale_members_event_event_id",
                        column: x => x.event_id,
                        principalSchema: "postgres",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_kids_scale_members_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kids_scale_members_event_id_member_id_role_name",
                schema: "postgres",
                table: "kids_scale_members",
                columns: new[] { "event_id", "member_id", "role_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_kids_scale_members_member_id",
                schema: "postgres",
                table: "kids_scale_members",
                column: "member_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kids_scale_members",
                schema: "postgres");
        }
    }
}
