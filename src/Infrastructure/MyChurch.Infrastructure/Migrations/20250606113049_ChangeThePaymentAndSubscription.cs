using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeThePaymentAndSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "subscription_id",
                schema: "postgres",
                table: "payment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "donation_id",
                schema: "postgres",
                table: "payment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_donation_id",
                schema: "postgres",
                table: "payment",
                column: "donation_id");

            migrationBuilder.AddForeignKey(
                name: "FK_payment_donation_donation_id",
                schema: "postgres",
                table: "payment",
                column: "donation_id",
                principalSchema: "postgres",
                principalTable: "donation",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payment_donation_donation_id",
                schema: "postgres",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "IX_payment_donation_id",
                schema: "postgres",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "donation_id",
                schema: "postgres",
                table: "payment");

            migrationBuilder.AlterColumn<int>(
                name: "subscription_id",
                schema: "postgres",
                table: "payment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
