using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTableEventNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "finish_date",
                schema: "postgres",
                table: "event",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "location",
                schema: "postgres",
                table: "event",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "requires_participant_list",
                schema: "postgres",
                table: "event",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "event_notification",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_notification", x => x.id);
                    table.ForeignKey(
                        name: "FK_event_notification_event_event_id",
                        column: x => x.event_id,
                        principalSchema: "postgres",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_notification_event_id",
                schema: "postgres",
                table: "event_notification",
                column: "event_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_notification",
                schema: "postgres");

            migrationBuilder.DropColumn(
                name: "finish_date",
                schema: "postgres",
                table: "event");

            migrationBuilder.DropColumn(
                name: "location",
                schema: "postgres",
                table: "event");

            migrationBuilder.DropColumn(
                name: "requires_participant_list",
                schema: "postgres",
                table: "event");
        }
    }
}
