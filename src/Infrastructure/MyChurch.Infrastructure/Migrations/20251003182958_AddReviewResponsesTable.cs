using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewResponsesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewId = table.Column<int>(type: "integer", nullable: false),
                    ResponderId = table.Column<int>(type: "int", nullable: false),
                    Response = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsEdited = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewResponses_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewResponses_member_ResponderId",
                        column: x => x.ResponderId,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewResponses_RespondedAt",
                table: "ReviewResponses",
                column: "RespondedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewResponses_ResponderId",
                table: "ReviewResponses",
                column: "ResponderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewResponses_ReviewId",
                table: "ReviewResponses",
                column: "ReviewId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewResponses");
        }
    }
}
