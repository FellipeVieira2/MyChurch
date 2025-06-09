using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatFormFeeInCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "platform_fee",
                schema: "postgres",
                table: "donation",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "platform_fee",
                schema: "postgres",
                table: "church",
                type: "numeric(5,4)",
                nullable: false,
                defaultValue: 0.05m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "platform_fee",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropColumn(
                name: "platform_fee",
                schema: "postgres",
                table: "church");
        }
    }
}
