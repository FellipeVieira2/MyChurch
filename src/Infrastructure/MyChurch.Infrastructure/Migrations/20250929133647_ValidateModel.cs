using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ValidateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "member_id",
                schema: "postgres",
                table: "worship_presences",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

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

            migrationBuilder.AddColumn<int>(
                name: "visitor_id",
                schema: "postgres",
                table: "worship_presences",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "member_id",
                schema: "postgres",
                table: "donation",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "visitor_id",
                schema: "postgres",
                table: "donation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                schema: "postgres",
                table: "church",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                schema: "postgres",
                table: "church",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "visitor",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    asaas_customer_id = table.Column<string>(type: "text", nullable: true),
                    score = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: true),
                    last_visit_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    needs_follow_up = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visitor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "visitor_status_history",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    visitor_id = table.Column<int>(type: "integer", nullable: false),
                    old_status = table.Column<int>(type: "integer", nullable: false),
                    new_status = table.Column<int>(type: "integer", nullable: false),
                    changed_by_member_id = table.Column<int>(type: "integer", nullable: true),
                    changed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visitor_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_visitor_status_history_visitor_visitor_id",
                        column: x => x.visitor_id,
                        principalSchema: "postgres",
                        principalTable: "visitor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_donation_visitor_id",
                schema: "postgres",
                table: "donation",
                column: "visitor_id");

            migrationBuilder.CreateIndex(
                name: "IX_visitor_status_history_visitor_id",
                schema: "postgres",
                table: "visitor_status_history",
                column: "visitor_id");

            migrationBuilder.AddForeignKey(
                name: "FK_donation_visitor_visitor_id",
                schema: "postgres",
                table: "donation",
                column: "visitor_id",
                principalSchema: "postgres",
                principalTable: "visitor",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_donation_visitor_visitor_id",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropTable(
                name: "visitor_status_history",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "visitor",
                schema: "postgres");

            migrationBuilder.DropIndex(
                name: "IX_donation_visitor_id",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropColumn(
                name: "latitude",
                schema: "postgres",
                table: "worship_presences");

            migrationBuilder.DropColumn(
                name: "longitude",
                schema: "postgres",
                table: "worship_presences");

            migrationBuilder.DropColumn(
                name: "visitor_id",
                schema: "postgres",
                table: "worship_presences");

            migrationBuilder.DropColumn(
                name: "visitor_id",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "Longitude",
                schema: "postgres",
                table: "church");

            migrationBuilder.AlterColumn<int>(
                name: "member_id",
                schema: "postgres",
                table: "worship_presences",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "member_id",
                schema: "postgres",
                table: "donation",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
