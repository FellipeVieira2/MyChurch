using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    public partial class AddVisitorStatusAndHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "score",
                schema: "postgres",
                table: "visitor",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "status",
                schema: "postgres",
                table: "visitor",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "needs_follow_up",
                schema: "postgres",
                table: "visitor",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_visit_at",
                schema: "postgres",
                table: "visitor",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "visitor_status_history",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    visitor_id = table.Column<int>(type: "integer", nullable: false),
                    old_status = table.Column<int>(type: "integer", nullable: false),
                    new_status = table.Column<int>(type: "integer", nullable: false),
                    changed_by_member_id = table.Column<int>(type: "integer", nullable: true),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visitor_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_visitor_status_history_visitor_visitor_id",
                        column: x => x.visitor_id,
                        principalSchema: "postgres",
                        principalTable: "visitor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_visitor_status_history_visitor_id",
                schema: "postgres",
                table: "visitor_status_history",
                column: "visitor_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "visitor_status_history",
                schema: "postgres");

            migrationBuilder.DropColumn(
                name: "score",
                schema: "postgres",
                table: "visitor");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "postgres",
                table: "visitor");

            migrationBuilder.DropColumn(
                name: "needs_follow_up",
                schema: "postgres",
                table: "visitor");

            migrationBuilder.DropColumn(
                name: "last_visit_at",
                schema: "postgres",
                table: "visitor");
        }
    }
}
