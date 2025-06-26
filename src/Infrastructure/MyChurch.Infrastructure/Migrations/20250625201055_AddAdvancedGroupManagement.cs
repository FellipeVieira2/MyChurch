using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancedGroupManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupMeetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    MeetingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Topic = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SummaryNotes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMeetings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMeetings_group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupMeetingAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupMeetingId = table.Column<int>(type: "integer", nullable: false),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    IsPresent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMeetingAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMeetingAttendances_GroupMeetings_GroupMeetingId",
                        column: x => x.GroupMeetingId,
                        principalTable: "GroupMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupMeetingMemberNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupMeetingId = table.Column<int>(type: "integer", nullable: false),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    LeaderId = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMeetingMemberNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMeetingMemberNotes_GroupMeetings_GroupMeetingId",
                        column: x => x.GroupMeetingId,
                        principalTable: "GroupMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupMeetingAttendances_GroupMeetingId",
                table: "GroupMeetingAttendances",
                column: "GroupMeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMeetingMemberNotes_GroupMeetingId",
                table: "GroupMeetingMemberNotes",
                column: "GroupMeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMeetings_GroupId",
                table: "GroupMeetings",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupMeetingAttendances");

            migrationBuilder.DropTable(
                name: "GroupMeetingMemberNotes");

            migrationBuilder.DropTable(
                name: "GroupMeetings");
        }
    }
}
