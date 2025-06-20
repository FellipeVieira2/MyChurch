using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFaithLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FaithLevelId",
                schema: "postgres",
                table: "member",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "faith_levels",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    points_required = table.Column<int>(type: "integer", nullable: false),
                    icon_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_faith_levels", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_member_FaithLevelId",
                schema: "postgres",
                table: "member",
                column: "FaithLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_member_faith_levels_FaithLevelId",
                schema: "postgres",
                table: "member",
                column: "FaithLevelId",
                principalTable: "faith_levels",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_member_faith_levels_FaithLevelId",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropTable(
                name: "faith_levels");

            migrationBuilder.DropIndex(
                name: "IX_member_FaithLevelId",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropColumn(
                name: "FaithLevelId",
                schema: "postgres",
                table: "member");
        }
    }
}
