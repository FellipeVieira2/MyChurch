using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentsAndDepartmentFinancial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "department_id",
                schema: "postgres",
                table: "worship_services",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                schema: "postgres",
                table: "event",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                schema: "postgres",
                table: "donation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                schema: "postgres",
                table: "cash_flow_entries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "departments",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    banking_info_id = table.Column<int>(type: "integer", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.id);
                    table.ForeignKey(
                        name: "FK_departments_banking_info_banking_info_id",
                        column: x => x.banking_info_id,
                        principalSchema: "postgres",
                        principalTable: "banking_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_departments_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "department_members",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    department_id = table.Column<int>(type: "integer", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    joined_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_department_members_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "postgres",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_department_members_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_worship_services_department_id",
                schema: "postgres",
                table: "worship_services",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_department_id",
                schema: "postgres",
                table: "event",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_donation_department_id",
                schema: "postgres",
                table: "donation",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_cash_flow_entries_department_id",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_department_members_department_id_member_id",
                schema: "postgres",
                table: "department_members",
                columns: new[] { "department_id", "member_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_department_members_member_id",
                schema: "postgres",
                table: "department_members",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_departments_banking_info_id",
                schema: "postgres",
                table: "departments",
                column: "banking_info_id");

            migrationBuilder.CreateIndex(
                name: "IX_departments_church_id_name",
                schema: "postgres",
                table: "departments",
                columns: new[] { "church_id", "name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_cash_flow_entries_departments_department_id",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "department_id",
                principalSchema: "postgres",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_donation_departments_department_id",
                schema: "postgres",
                table: "donation",
                column: "department_id",
                principalSchema: "postgres",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_event_departments_department_id",
                schema: "postgres",
                table: "event",
                column: "department_id",
                principalSchema: "postgres",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_worship_services_departments_department_id",
                schema: "postgres",
                table: "worship_services",
                column: "department_id",
                principalSchema: "postgres",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cash_flow_entries_departments_department_id",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_donation_departments_department_id",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropForeignKey(
                name: "FK_event_departments_department_id",
                schema: "postgres",
                table: "event");

            migrationBuilder.DropForeignKey(
                name: "FK_transfer_history_banking_info_banking_info_id",
                schema: "postgres",
                table: "transfer_history");

            migrationBuilder.DropForeignKey(
                name: "FK_worship_services_departments_department_id",
                schema: "postgres",
                table: "worship_services");

            migrationBuilder.DropTable(
                name: "department_members",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "departments",
                schema: "postgres");

            migrationBuilder.DropIndex(
                name: "IX_worship_services_department_id",
                schema: "postgres",
                table: "worship_services");

            migrationBuilder.DropIndex(
                name: "IX_transfer_history_banking_info_id",
                schema: "postgres",
                table: "transfer_history");

            migrationBuilder.DropIndex(
                name: "IX_event_department_id",
                schema: "postgres",
                table: "event");

            migrationBuilder.DropIndex(
                name: "IX_donation_department_id",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropIndex(
                name: "IX_cash_flow_entries_department_id",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropColumn(
                name: "department_id",
                schema: "postgres",
                table: "worship_services");

            migrationBuilder.DropColumn(
                name: "banking_info_id",
                schema: "postgres",
                table: "transfer_history");

            migrationBuilder.DropColumn(
                name: "failure_reason",
                schema: "postgres",
                table: "transfer_history");

            migrationBuilder.DropColumn(
                name: "scheduled_for",
                schema: "postgres",
                table: "transfer_history");

            migrationBuilder.DropColumn(
                name: "department_id",
                schema: "postgres",
                table: "event");

            migrationBuilder.DropColumn(
                name: "department_id",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropColumn(
                name: "department_id",
                schema: "postgres",
                table: "cash_flow_entries");
        }
    }
}
