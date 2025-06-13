using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDonationWorshipServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "donation_worship_service",
                schema: "postgres",
                columns: table => new
                {
                    donation_id = table.Column<int>(type: "int", nullable: false),
                    worship_service_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donation_worship_service", x => new { x.donation_id, x.worship_service_id });
                    table.ForeignKey(
                        name: "FK_donation_worship_service_donation_donation_id",
                        column: x => x.donation_id,
                        principalSchema: "postgres",
                        principalTable: "donation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_donation_worship_service_worship_services_worship_service_id",
                        column: x => x.worship_service_id,
                        principalSchema: "postgres",
                        principalTable: "worship_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_donation_worship_service_worship_service_id",
                schema: "postgres",
                table: "donation_worship_service",
                column: "worship_service_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "donation_worship_service",
                schema: "postgres");
        }
    }
}
