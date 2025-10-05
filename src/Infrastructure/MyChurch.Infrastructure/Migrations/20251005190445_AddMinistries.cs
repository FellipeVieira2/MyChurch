using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMinistries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ministries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LeaderId = table.Column<int>(type: "int", nullable: true),
                    ChurchId = table.Column<int>(type: "int", nullable: false),
                    Photo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MeetingDay = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MeetingTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    MeetingLocation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ministries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ministries_church_ChurchId",
                        column: x => x.ChurchId,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ministries_member_LeaderId",
                        column: x => x.LeaderId,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MinistryMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MinistryId = table.Column<int>(type: "integer", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    JoinedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinistryMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinistryMembers_Ministries_MinistryId",
                        column: x => x.MinistryId,
                        principalTable: "Ministries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinistryMembers_member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ministries_ChurchId",
                table: "Ministries",
                column: "ChurchId");

            migrationBuilder.CreateIndex(
                name: "IX_Ministries_IsActive",
                table: "Ministries",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Ministries_LeaderId",
                table: "Ministries",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_MinistryMembers_IsActive",
                table: "MinistryMembers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MinistryMembers_MemberId",
                table: "MinistryMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MinistryMembers_MinistryId",
                table: "MinistryMembers",
                column: "MinistryId");

            migrationBuilder.CreateIndex(
                name: "IX_MinistryMembers_MinistryId_MemberId",
                table: "MinistryMembers",
                columns: new[] { "MinistryId", "MemberId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MinistryMembers");

            migrationBuilder.DropTable(
                name: "Ministries");
        }
    }
}
