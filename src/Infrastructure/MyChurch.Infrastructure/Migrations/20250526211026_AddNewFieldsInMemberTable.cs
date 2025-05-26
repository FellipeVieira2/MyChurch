using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsInMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "postgres",
                table: "member",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "marital_status",
                schema: "postgres",
                table: "member",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "member_since",
                schema: "postgres",
                table: "member",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ministry",
                schema: "postgres",
                table: "member",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                schema: "postgres",
                table: "member",
                type: "varchar(1000)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropColumn(
                name: "marital_status",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropColumn(
                name: "member_since",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropColumn(
                name: "ministry",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropColumn(
                name: "notes",
                schema: "postgres",
                table: "member");
        }
    }
}
