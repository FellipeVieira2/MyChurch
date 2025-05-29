using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTablesToControlTheFeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "feed_posts",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feed_posts", x => x.id);
                    table.ForeignKey(
                        name: "FK_feed_posts_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_feed_posts_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feed_likes",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    feed_post_id = table.Column<int>(type: "integer", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feed_likes", x => x.id);
                    table.ForeignKey(
                        name: "FK_feed_likes_feed_posts_feed_post_id",
                        column: x => x.feed_post_id,
                        principalSchema: "postgres",
                        principalTable: "feed_posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_feed_likes_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_feed_likes_feed_post_id_member_id",
                schema: "postgres",
                table: "feed_likes",
                columns: new[] { "feed_post_id", "member_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_feed_likes_member_id",
                schema: "postgres",
                table: "feed_likes",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_feed_posts_church_id",
                schema: "postgres",
                table: "feed_posts",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_feed_posts_member_id",
                schema: "postgres",
                table: "feed_posts",
                column: "member_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "feed_likes",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "feed_posts",
                schema: "postgres");
        }
    }
}
