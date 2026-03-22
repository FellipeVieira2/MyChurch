using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    public partial class AddWorshipScaleMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "worship_scale_members",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worship_service_id = table.Column<int>(type: "integer", nullable: false),
                    member_id = table.Column<int>(type: "integer", nullable: false),
                    role_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worship_scale_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_worship_scale_members_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_worship_scale_members_worship_services_worship_service_id",
                        column: x => x.worship_service_id,
                        principalSchema: "postgres",
                        principalTable: "worship_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_worship_scale_members_member_id",
                schema: "postgres",
                table: "worship_scale_members",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_worship_scale_members_worship_service_id_member_id_role_name",
                schema: "postgres",
                table: "worship_scale_members",
                columns: new[] { "worship_service_id", "member_id", "role_name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "worship_scale_members",
                schema: "postgres");
        }
    }
}
