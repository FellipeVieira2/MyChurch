using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCashFlowIntegrationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                schema: "postgres",
                table: "cash_flow_entries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DonationId",
                schema: "postgres",
                table: "cash_flow_entries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutomatic",
                schema: "postgres",
                table: "cash_flow_entries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReconciled",
                schema: "postgres",
                table: "cash_flow_entries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "postgres",
                table: "cash_flow_entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptUrl",
                schema: "postgres",
                table: "cash_flow_entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReconciledAt",
                schema: "postgres",
                table: "cash_flow_entries",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_cash_flow_entries_AssetId",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_cash_flow_entries_DonationId",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "DonationId");

            migrationBuilder.AddForeignKey(
                name: "FK_cash_flow_entries_asset_AssetId",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "AssetId",
                principalSchema: "postgres",
                principalTable: "asset",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_cash_flow_entries_donation_DonationId",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "DonationId",
                principalSchema: "postgres",
                principalTable: "donation",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cash_flow_entries_asset_AssetId",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_cash_flow_entries_donation_DonationId",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropIndex(
                name: "IX_cash_flow_entries_AssetId",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropIndex(
                name: "IX_cash_flow_entries_DonationId",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropColumn(
                name: "AssetId",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropColumn(
                name: "DonationId",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropColumn(
                name: "IsAutomatic",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropColumn(
                name: "IsReconciled",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropColumn(
                name: "ReceiptUrl",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropColumn(
                name: "ReconciledAt",
                schema: "postgres",
                table: "cash_flow_entries");
        }
    }
}
