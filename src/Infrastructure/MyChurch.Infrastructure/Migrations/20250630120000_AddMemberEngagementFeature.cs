using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyChurch.Infrastructure.Migrations
{
    public partial class AddMemberEngagementFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "engagement_score",
                schema: "postgres",
                table: "member",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "engagement_event",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    points = table.Column<int>(type: "int", nullable: false),
                    event_type = table.Column<int>(type: "int", nullable: false),
                    event_reference_id = table.Column<string>(type: "varchar(255)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engagement_event", x => x.id);
                    table.ForeignKey(
                        name: "FK_engagement_event_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_engagement_event_member_id_created_at",
                schema: "postgres",
                table: "engagement_event",
                columns: new[] { "member_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_engagement_event_church_id",
                schema: "postgres",
                table: "engagement_event",
                column: "church_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "engagement_event",
                schema: "postgres");

            migrationBuilder.DropColumn(
                name: "engagement_score",
                schema: "postgres",
                table: "member");
        }
    }
}
