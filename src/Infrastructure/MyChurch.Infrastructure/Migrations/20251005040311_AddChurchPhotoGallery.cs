using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChurchPhotoGallery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "church_photos",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    uploaded_by_member_id = table.Column<int>(type: "int", nullable: true),
                    uploaded_by_visitor_id = table.Column<int>(type: "int", nullable: true),
                    photo_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    original_file_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    caption = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    category = table.Column<int>(type: "int", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    likes = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_rejected = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    approved_by_admin_id = table.Column<int>(type: "int", nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    rejection_reason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    is_featured = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    display_order = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_church_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK_church_photos_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_church_photos_member_approved_by_admin_id",
                        column: x => x.approved_by_admin_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_church_photos_member_uploaded_by_member_id",
                        column: x => x.uploaded_by_member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_church_photos_visitor_uploaded_by_visitor_id",
                        column: x => x.uploaded_by_visitor_id,
                        principalSchema: "postgres",
                        principalTable: "visitor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "church_photo_likes",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_photo_id = table.Column<int>(type: "int", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: true),
                    visitor_id = table.Column<int>(type: "int", nullable: true),
                    liked_at = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_church_photo_likes", x => x.id);
                    table.ForeignKey(
                        name: "FK_church_photo_likes_church_photos_church_photo_id",
                        column: x => x.church_photo_id,
                        principalSchema: "postgres",
                        principalTable: "church_photos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_church_photo_likes_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_church_photo_likes_visitor_visitor_id",
                        column: x => x.visitor_id,
                        principalSchema: "postgres",
                        principalTable: "visitor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_church_photo_likes_member_id",
                schema: "postgres",
                table: "church_photo_likes",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_church_photo_likes_photo_member",
                schema: "postgres",
                table: "church_photo_likes",
                columns: new[] { "church_photo_id", "member_id" },
                unique: true,
                filter: "member_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_church_photo_likes_photo_visitor",
                schema: "postgres",
                table: "church_photo_likes",
                columns: new[] { "church_photo_id", "visitor_id" },
                unique: true,
                filter: "visitor_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_church_photo_likes_visitor_id",
                schema: "postgres",
                table: "church_photo_likes",
                column: "visitor_id");

            migrationBuilder.CreateIndex(
                name: "IX_church_photos_approved_by_admin_id",
                schema: "postgres",
                table: "church_photos",
                column: "approved_by_admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_church_photos_church_approved_category",
                schema: "postgres",
                table: "church_photos",
                columns: new[] { "church_id", "is_approved", "category" });

            migrationBuilder.CreateIndex(
                name: "IX_church_photos_featured_order",
                schema: "postgres",
                table: "church_photos",
                columns: new[] { "is_featured", "display_order" });

            migrationBuilder.CreateIndex(
                name: "IX_church_photos_uploaded_at",
                schema: "postgres",
                table: "church_photos",
                column: "uploaded_at");

            migrationBuilder.CreateIndex(
                name: "IX_church_photos_uploaded_by_member_id",
                schema: "postgres",
                table: "church_photos",
                column: "uploaded_by_member_id");

            migrationBuilder.CreateIndex(
                name: "IX_church_photos_uploaded_by_visitor_id",
                schema: "postgres",
                table: "church_photos",
                column: "uploaded_by_visitor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "church_photo_likes",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "church_photos",
                schema: "postgres");
        }
    }
}
