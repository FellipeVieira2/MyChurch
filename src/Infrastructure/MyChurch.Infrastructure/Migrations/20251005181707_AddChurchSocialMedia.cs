using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChurchSocialMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "postgres",
                table: "church",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                schema: "postgres",
                table: "church",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                schema: "postgres",
                table: "church",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TikTokUrl",
                schema: "postgres",
                table: "church",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitterUrl",
                schema: "postgres",
                table: "church",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                schema: "postgres",
                table: "church",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhatsAppNumber",
                schema: "postgres",
                table: "church",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeUrl",
                schema: "postgres",
                table: "church",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "TikTokUrl",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "TwitterUrl",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "Website",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "WhatsAppNumber",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "YoutubeUrl",
                schema: "postgres",
                table: "church");
        }
    }
}
