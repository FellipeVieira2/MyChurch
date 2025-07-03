using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOnboardingQrCodeToChurch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "onboarding_qrcode",
                schema: "postgres",
                table: "church",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "onboarding_qrcode",
                schema: "postgres",
                table: "church");
        }
    }
}
