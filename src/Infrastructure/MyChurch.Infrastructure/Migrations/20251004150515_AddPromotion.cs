using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "can_promote_church",
                schema: "postgres",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_promote_events",
                schema: "postgres",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "has_advanced_analytics",
                schema: "postgres",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "has_priority_support",
                schema: "postgres",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "has_verified_badge",
                schema: "postgres",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "shows_ads",
                schema: "postgres",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "church_promotions",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    amount_paid = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    target_region = table.Column<string>(type: "varchar(100)", nullable: true),
                    custom_banner_url = table.Column<string>(type: "varchar(500)", nullable: true),
                    custom_text = table.Column<string>(type: "varchar(500)", nullable: true),
                    views = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    clicks = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    payment_id = table.Column<int>(type: "int", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_church_promotions", x => x.id);
                    table.ForeignKey(
                        name: "FK_church_promotions_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_church_promotions_payment_payment_id",
                        column: x => x.payment_id,
                        principalSchema: "postgres",
                        principalTable: "payment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "event_promotions",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    budget = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    amount_spent = table.Column<decimal>(type: "numeric(10,2)", nullable: false, defaultValue: 0m),
                    status = table.Column<int>(type: "int", nullable: false),
                    target_region = table.Column<string>(type: "varchar(100)", nullable: true),
                    target_radius_km = table.Column<double>(type: "double precision", nullable: true),
                    custom_banner_url = table.Column<string>(type: "varchar(500)", nullable: true),
                    views = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    clicks = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    conversions = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    payment_id = table.Column<int>(type: "int", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_promotions", x => x.id);
                    table.ForeignKey(
                        name: "FK_event_promotions_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_promotions_event_event_id",
                        column: x => x.event_id,
                        principalSchema: "postgres",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_promotions_payment_payment_id",
                        column: x => x.payment_id,
                        principalSchema: "postgres",
                        principalTable: "payment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_church_promotions_church_id",
                schema: "postgres",
                table: "church_promotions",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_church_promotions_payment_id",
                schema: "postgres",
                table: "church_promotions",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_church_promotions_status_dates",
                schema: "postgres",
                table: "church_promotions",
                columns: new[] { "status", "start_date", "end_date" });

            migrationBuilder.CreateIndex(
                name: "IX_church_promotions_type",
                schema: "postgres",
                table: "church_promotions",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_event_promotions_church_id",
                schema: "postgres",
                table: "event_promotions",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_promotions_event_id",
                schema: "postgres",
                table: "event_promotions",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_promotions_payment_id",
                schema: "postgres",
                table: "event_promotions",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_promotions_status_dates",
                schema: "postgres",
                table: "event_promotions",
                columns: new[] { "status", "start_date", "end_date" });

            migrationBuilder.CreateIndex(
                name: "IX_event_promotions_type",
                schema: "postgres",
                table: "event_promotions",
                column: "type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "church_promotions",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "event_promotions",
                schema: "postgres");

            migrationBuilder.DropColumn(
                name: "can_promote_church",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "can_promote_events",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "has_advanced_analytics",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "has_priority_support",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "has_verified_badge",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "shows_ads",
                schema: "postgres",
                table: "plan");
        }
    }
}
