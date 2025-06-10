using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBankingInfos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "banking_info",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    bank_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    agency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    account = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    account_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    holder_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    holder_document = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    pix_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    pix_key_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banking_info", x => x.id);
                    table.ForeignKey(
                        name: "FK_banking_info_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_banking_info_church_id",
                schema: "postgres",
                table: "banking_info",
                column: "church_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "banking_info",
                schema: "postgres");
        }
    }
}
