using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTheTablesToSUpportMultipleDocumentsinMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cash_flow_entries_member_member_id",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropColumn(
                name: "document",
                schema: "postgres",
                table: "member");

            migrationBuilder.AlterColumn<bool>(
                name: "is_tither",
                schema: "postgres",
                table: "member",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "BirthCity",
                schema: "postgres",
                table: "member",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirthState",
                schema: "postgres",
                table: "member",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "address_id",
                schema: "postgres",
                table: "member",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Complement",
                schema: "postgres",
                table: "address",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "member_document",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    number = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_document", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_document_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_member_address_id",
                schema: "postgres",
                table: "member",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_document_member_id",
                schema: "postgres",
                table: "member_document",
                column: "member_id");

            migrationBuilder.AddForeignKey(
                name: "FK_cash_flow_entries_member_member_id",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "member_id",
                principalSchema: "postgres",
                principalTable: "member",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_member_address_address_id",
                schema: "postgres",
                table: "member",
                column: "address_id",
                principalSchema: "postgres",
                principalTable: "address",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cash_flow_entries_member_member_id",
                schema: "postgres",
                table: "cash_flow_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_member_address_address_id",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropTable(
                name: "member_document",
                schema: "postgres");

            migrationBuilder.DropIndex(
                name: "IX_member_address_id",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropColumn(
                name: "BirthCity",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropColumn(
                name: "BirthState",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropColumn(
                name: "address_id",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropColumn(
                name: "Complement",
                schema: "postgres",
                table: "address");

            migrationBuilder.AlterColumn<bool>(
                name: "is_tither",
                schema: "postgres",
                table: "member",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "document",
                schema: "postgres",
                table: "member",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_cash_flow_entries_member_member_id",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "member_id",
                principalSchema: "postgres",
                principalTable: "member",
                principalColumn: "id");
        }
    }
}
