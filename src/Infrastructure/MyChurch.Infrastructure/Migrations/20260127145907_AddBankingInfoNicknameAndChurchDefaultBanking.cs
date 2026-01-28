using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBankingInfoNicknameAndChurchDefaultBanking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "default_banking_info_id",
                schema: "postgres",
                table: "church",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nickname",
                schema: "postgres",
                table: "banking_info",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_church_default_banking_info_id",
                schema: "postgres",
                table: "church",
                column: "default_banking_info_id");

            migrationBuilder.AddForeignKey(
                name: "FK_church_banking_info_default_banking_info_id",
                schema: "postgres",
                table: "church",
                column: "default_banking_info_id",
                principalSchema: "postgres",
                principalTable: "banking_info",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_church_banking_info_default_banking_info_id",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropIndex(
                name: "IX_church_default_banking_info_id",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "default_banking_info_id",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "nickname",
                schema: "postgres",
                table: "banking_info");
        }
    }
}
