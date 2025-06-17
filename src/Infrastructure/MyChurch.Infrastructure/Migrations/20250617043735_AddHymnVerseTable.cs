using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHymnVerseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "lyrics",
                schema: "postgres",
                table: "hymns",
                newName: "chorus");

            migrationBuilder.CreateTable(
                name: "hymn_verses",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hymn_id = table.Column<int>(type: "integer", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hymn_verses", x => x.id);
                    table.ForeignKey(
                        name: "FK_hymn_verses_hymns_hymn_id",
                        column: x => x.hymn_id,
                        principalSchema: "postgres",
                        principalTable: "hymns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hymn_verses_hymn_id",
                schema: "postgres",
                table: "hymn_verses",
                column: "hymn_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hymn_verses",
                schema: "postgres");

            migrationBuilder.RenameColumn(
                name: "chorus",
                schema: "postgres",
                table: "hymns",
                newName: "lyrics");
        }
    }
}
