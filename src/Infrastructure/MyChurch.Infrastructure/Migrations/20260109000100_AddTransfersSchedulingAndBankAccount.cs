using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    public partial class AddTransfersSchedulingAndBankAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "banking_info_id",
                schema: "postgres",
                table: "transfer_history",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "scheduled_for",
                schema: "postgres",
                table: "transfer_history",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "failure_reason",
                schema: "postgres",
                table: "transfer_history",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transfer_history_banking_info_id",
                schema: "postgres",
                table: "transfer_history",
                column: "banking_info_id");

            migrationBuilder.AddForeignKey(
                name: "FK_transfer_history_banking_info_banking_info_id",
                schema: "postgres",
                table: "transfer_history",
                column: "banking_info_id",
                principalSchema: "postgres",
                principalTable: "banking_info",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transfer_history_banking_info_banking_info_id",
                schema: "postgres",
                table: "transfer_history");

            migrationBuilder.DropIndex(
                name: "IX_transfer_history_banking_info_id",
                schema: "postgres",
                table: "transfer_history");

            migrationBuilder.DropColumn(
                name: "banking_info_id",
                schema: "postgres",
                table: "transfer_history");

            migrationBuilder.DropColumn(
                name: "scheduled_for",
                schema: "postgres",
                table: "transfer_history");

            migrationBuilder.DropColumn(
                name: "failure_reason",
                schema: "postgres",
                table: "transfer_history");
        }
    }
}
