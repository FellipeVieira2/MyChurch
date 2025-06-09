using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustColumnsInMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BirthState",
                schema: "postgres",
                table: "member",
                newName: "birth_state");

            migrationBuilder.RenameColumn(
                name: "BirthCity",
                schema: "postgres",
                table: "member",
                newName: "birth_city");

            migrationBuilder.AlterColumn<string>(
                name: "birth_state",
                schema: "postgres",
                table: "member",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "birth_city",
                schema: "postgres",
                table: "member",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "birth_state",
                schema: "postgres",
                table: "member",
                newName: "BirthState");

            migrationBuilder.RenameColumn(
                name: "birth_city",
                schema: "postgres",
                table: "member",
                newName: "BirthCity");

            migrationBuilder.AlterColumn<string>(
                name: "BirthState",
                schema: "postgres",
                table: "member",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BirthCity",
                schema: "postgres",
                table: "member",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);
        }
    }
}
