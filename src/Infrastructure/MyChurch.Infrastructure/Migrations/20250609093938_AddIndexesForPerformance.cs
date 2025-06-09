using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    public partial class AddIndexesForPerformance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_payment_transaction_id",
                schema: "postgres",
                table: "payment",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_payment_status",
                schema: "postgres",
                table: "payment",
                column: "payment_status");

            migrationBuilder.CreateIndex(
                name: "IX_member_document_type_number",
                schema: "postgres",
                table: "member_document",
                columns: new[] { "type", "number" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_payment_transaction_id",
                schema: "postgres",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "IX_payment_payment_status",
                schema: "postgres",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "IX_member_document_type_number",
                schema: "postgres",
                table: "member_document");
        }
    }
}