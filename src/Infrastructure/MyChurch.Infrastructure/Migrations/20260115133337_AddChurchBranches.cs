using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    public partial class AddChurchBranches : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "parent_church_id",
                schema: "postgres",
                table: "church",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Church_ParentChurchId",
                schema: "postgres",
                table: "church",
                column: "parent_church_id");

            migrationBuilder.AddForeignKey(
                name: "FK_church_church_parent_church_id",
                schema: "postgres",
                table: "church",
                column: "parent_church_id",
                principalSchema: "postgres",
                principalTable: "church",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_church_church_parent_church_id",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropIndex(
                name: "IX_Church_ParentChurchId",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "parent_church_id",
                schema: "postgres",
                table: "church");
        }
    }
}
