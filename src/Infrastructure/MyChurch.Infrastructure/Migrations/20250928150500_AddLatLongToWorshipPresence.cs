using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    public partial class AddLatLongToWorshipPresence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "latitude",
                schema: "postgres",
                table: "worship_presences",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "longitude",
                schema: "postgres",
                table: "worship_presences",
                type: "double precision",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "latitude",
                schema: "postgres",
                table: "worship_presences");

            migrationBuilder.DropColumn(
                name: "longitude",
                schema: "postgres",
                table: "worship_presences");
        }
    }
}
