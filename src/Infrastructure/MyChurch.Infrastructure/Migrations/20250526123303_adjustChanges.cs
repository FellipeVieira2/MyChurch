using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adjustChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Branches",
                schema: "postgres",
                table: "plan",
                newName: "branches");

            migrationBuilder.AlterColumn<int>(
                name: "branches",
                schema: "postgres",
                table: "plan",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "branches",
                schema: "postgres",
                table: "plan",
                newName: "Branches");

            migrationBuilder.AlterColumn<int>(
                name: "Branches",
                schema: "postgres",
                table: "plan",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
