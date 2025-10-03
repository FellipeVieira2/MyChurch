using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewVerificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "Reviews",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorshipPresenceId",
                table: "Reviews",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_WorshipPresenceId",
                table: "Reviews",
                column: "WorshipPresenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_worship_presences_WorshipPresenceId",
                table: "Reviews",
                column: "WorshipPresenceId",
                principalSchema: "postgres",
                principalTable: "worship_presences",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_worship_presences_WorshipPresenceId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_WorshipPresenceId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "WorshipPresenceId",
                table: "Reviews");
        }
    }
}
