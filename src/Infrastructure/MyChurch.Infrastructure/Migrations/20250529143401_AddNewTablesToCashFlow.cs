using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTablesToCashFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cash_flow_categories",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    church_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cash_flow_categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_cash_flow_categories_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cash_flow_entries",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cash_flow_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_cash_flow_entries_cash_flow_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "postgres",
                        principalTable: "cash_flow_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cash_flow_entries_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cash_flow_entries_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_cash_flow_categories_church_id",
                schema: "postgres",
                table: "cash_flow_categories",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_cash_flow_entries_category_id",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_cash_flow_entries_church_id",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_cash_flow_entries_member_id",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "member_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cash_flow_entries",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "cash_flow_categories",
                schema: "postgres");
        }
    }
}
