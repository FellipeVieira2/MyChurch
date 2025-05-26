using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsInAssetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "condition",
                schema: "postgres",
                table: "asset",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_maintenance",
                schema: "postgres",
                table: "asset",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "location",
                schema: "postgres",
                table: "asset",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "next_maintenance",
                schema: "postgres",
                table: "asset",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                schema: "postgres",
                table: "asset",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "purchase_date",
                schema: "postgres",
                table: "asset",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "responsible",
                schema: "postgres",
                table: "asset",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "warranty_until",
                schema: "postgres",
                table: "asset",
                type: "timestamp",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "condition",
                schema: "postgres",
                table: "asset");

            migrationBuilder.DropColumn(
                name: "last_maintenance",
                schema: "postgres",
                table: "asset");

            migrationBuilder.DropColumn(
                name: "location",
                schema: "postgres",
                table: "asset");

            migrationBuilder.DropColumn(
                name: "next_maintenance",
                schema: "postgres",
                table: "asset");

            migrationBuilder.DropColumn(
                name: "notes",
                schema: "postgres",
                table: "asset");

            migrationBuilder.DropColumn(
                name: "purchase_date",
                schema: "postgres",
                table: "asset");

            migrationBuilder.DropColumn(
                name: "responsible",
                schema: "postgres",
                table: "asset");

            migrationBuilder.DropColumn(
                name: "warranty_until",
                schema: "postgres",
                table: "asset");
        }
    }
}
