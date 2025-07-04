using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adjustWorshipActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "verse_number",
                schema: "postgres",
                table: "worship_activity_hymns",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "donation_time",
                schema: "postgres",
                table: "worship_activities",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "verse_number",
                schema: "postgres",
                table: "worship_activity_hymns");

            migrationBuilder.DropColumn(
                name: "donation_time",
                schema: "postgres",
                table: "worship_activities");
        }
    }
}
