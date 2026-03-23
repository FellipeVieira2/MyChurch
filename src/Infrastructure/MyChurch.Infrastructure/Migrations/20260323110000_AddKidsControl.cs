using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    public partial class AddKidsControl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "child_pickup_authorizations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    child_id = table.Column<int>(type: "integer", nullable: false),
                    full_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    relationship = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    document_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_member_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_child_pickup_authorizations", x => x.id);
                    table.ForeignKey(
                        name: "FK_child_pickup_authorizations_child_child_id",
                        column: x => x.child_id,
                        principalTable: "child",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_child_pickup_authorizations_member_created_by_member_id",
                        column: x => x.created_by_member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "kids_check_ins",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    child_id = table.Column<int>(type: "integer", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    checked_in_by_member_id = table.Column<int>(type: "int", nullable: false),
                    checked_in_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    environment_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    pickup_token = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pickup_token_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    checked_out_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    checked_out_by_member_id = table.Column<int>(type: "int", nullable: true),
                    authorized_pickup_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kids_check_ins", x => x.id);
                    table.ForeignKey(
                        name: "FK_kids_check_ins_child_pickup_authorizations_authorized_pickup_id",
                        column: x => x.authorized_pickup_id,
                        principalTable: "child_pickup_authorizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_kids_check_ins_child_child_id",
                        column: x => x.child_id,
                        principalTable: "child",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_kids_check_ins_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_kids_check_ins_member_checked_in_by_member_id",
                        column: x => x.checked_in_by_member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_kids_check_ins_member_checked_out_by_member_id",
                        column: x => x.checked_out_by_member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_child_pickup_authorizations_child_id",
                table: "child_pickup_authorizations",
                column: "child_id");

            migrationBuilder.CreateIndex(
                name: "IX_child_pickup_authorizations_created_by_member_id",
                table: "child_pickup_authorizations",
                column: "created_by_member_id");

            migrationBuilder.CreateIndex(
                name: "IX_child_pickup_authorizations_child_id_full_name_phone_number",
                table: "child_pickup_authorizations",
                columns: new[] { "child_id", "full_name", "phone_number" });

            migrationBuilder.CreateIndex(
                name: "IX_kids_check_ins_authorized_pickup_id",
                table: "kids_check_ins",
                column: "authorized_pickup_id");

            migrationBuilder.CreateIndex(
                name: "IX_kids_check_ins_checked_in_by_member_id",
                table: "kids_check_ins",
                column: "checked_in_by_member_id");

            migrationBuilder.CreateIndex(
                name: "IX_kids_check_ins_checked_out_by_member_id",
                table: "kids_check_ins",
                column: "checked_out_by_member_id");

            migrationBuilder.CreateIndex(
                name: "IX_kids_check_ins_child_active",
                table: "kids_check_ins",
                column: "child_id",
                unique: true,
                filter: "checked_out_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_kids_check_ins_church_checked_in_at",
                table: "kids_check_ins",
                columns: new[] { "church_id", "checked_in_at" });

            migrationBuilder.CreateIndex(
                name: "IX_kids_check_ins_pickup_token",
                table: "kids_check_ins",
                column: "pickup_token",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kids_check_ins");

            migrationBuilder.DropTable(
                name: "child_pickup_authorizations");
        }
    }
}
