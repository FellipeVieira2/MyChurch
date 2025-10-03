using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    public partial class AddVisitorAndDonationVisitor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "visitor",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visitor", x => x.id);
                });

            migrationBuilder.AddColumn<int>(
                name: "visitor_id",
                schema: "postgres",
                table: "donation",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_donation_visitor_visitor_id",
                schema: "postgres",
                table: "donation",
                column: "visitor_id",
                principalSchema: "postgres",
                principalTable: "visitor",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_donation_visitor_visitor_id",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropColumn(
                name: "visitor_id",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropTable(
                name: "visitor",
                schema: "postgres");
        }
    }
}
