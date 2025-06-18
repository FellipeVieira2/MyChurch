using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaingsTableAndColumninMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PendingApproval",
                schema: "postgres",
                table: "member",
                newName: "pending_approval");

            migrationBuilder.AddColumn<int>(
                name: "campaign_id",
                schema: "postgres",
                table: "donation",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "campaigns",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "int", maxLength: 300, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    goal_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    amount_raised = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    cover_image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaigns", x => x.id);
                    table.ForeignKey(
                        name: "FK_campaigns_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_donation_campaign_id",
                schema: "postgres",
                table: "donation",
                column: "campaign_id");

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_church_id",
                table: "campaigns",
                column: "church_id");

            migrationBuilder.AddForeignKey(
                name: "FK_donation_campaigns_campaign_id",
                schema: "postgres",
                table: "donation",
                column: "campaign_id",
                principalTable: "campaigns",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_donation_campaigns_campaign_id",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropTable(
                name: "campaigns");

            migrationBuilder.DropIndex(
                name: "IX_donation_campaign_id",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropColumn(
                name: "campaign_id",
                schema: "postgres",
                table: "donation");

            migrationBuilder.RenameColumn(
                name: "pending_approval",
                schema: "postgres",
                table: "member",
                newName: "PendingApproval");
        }
    }
}
