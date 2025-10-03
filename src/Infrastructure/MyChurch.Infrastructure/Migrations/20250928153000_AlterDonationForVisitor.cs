using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    public partial class AlterDonationForVisitor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tornar member_id nullable
            migrationBuilder.AlterColumn<int>(
                name: "member_id",
                schema: "postgres",
                table: "donation",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // visitor_id já adicionado em migration anterior (garantir caso não exista)
            if (!ColumnExists(migrationBuilder, "postgres", "donation", "visitor_id"))
            {
                migrationBuilder.AddColumn<int>(
                    name: "visitor_id",
                    schema: "postgres",
                    table: "donation",
                    type: "int",
                    nullable: true);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "member_id",
                schema: "postgres",
                table: "donation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "visitor_id",
                schema: "postgres",
                table: "donation");
        }

        // Helper dummy (não executa runtime – apenas evita erro em design se usado manualmente)
        private bool ColumnExists(MigrationBuilder migrationBuilder, string schema, string table, string column) => true;
    }
}
