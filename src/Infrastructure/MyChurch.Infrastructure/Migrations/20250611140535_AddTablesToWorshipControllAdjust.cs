using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesToWorshipControllAdjust : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_worship_services_WorshipServiceId",
                schema: "postgres",
                table: "event");

            migrationBuilder.DropIndex(
                name: "IX_event_WorshipServiceId",
                schema: "postgres",
                table: "event");

            migrationBuilder.RenameColumn(
                name: "WorshipServiceId",
                schema: "postgres",
                table: "event",
                newName: "worship_service_id");

            migrationBuilder.RenameColumn(
                name: "EventType",
                schema: "postgres",
                table: "event",
                newName: "event_type");

            migrationBuilder.AddColumn<int>(
                name: "event_id",
                schema: "postgres",
                table: "worship_services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_worship_services_event_id",
                schema: "postgres",
                table: "worship_services",
                column: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK_worship_services_event_event_id",
                schema: "postgres",
                table: "worship_services",
                column: "event_id",
                principalSchema: "postgres",
                principalTable: "event",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_worship_services_event_event_id",
                schema: "postgres",
                table: "worship_services");

            migrationBuilder.DropIndex(
                name: "IX_worship_services_event_id",
                schema: "postgres",
                table: "worship_services");

            migrationBuilder.DropColumn(
                name: "event_id",
                schema: "postgres",
                table: "worship_services");

            migrationBuilder.RenameColumn(
                name: "worship_service_id",
                schema: "postgres",
                table: "event",
                newName: "WorshipServiceId");

            migrationBuilder.RenameColumn(
                name: "event_type",
                schema: "postgres",
                table: "event",
                newName: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_event_WorshipServiceId",
                schema: "postgres",
                table: "event",
                column: "WorshipServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_event_worship_services_WorshipServiceId",
                schema: "postgres",
                table: "event",
                column: "WorshipServiceId",
                principalSchema: "postgres",
                principalTable: "worship_services",
                principalColumn: "id");
        }
    }
}
