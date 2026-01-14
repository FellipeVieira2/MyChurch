using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    public partial class AddPlatformUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "platform_user",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(200)", nullable: false),
                    email = table.Column<string>(type: "varchar(200)", nullable: false),
                    photo = table.Column<string>(type: "varchar(500)", nullable: true),
                    password_hash = table.Column<string>(type: "varchar(500)", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    role = table.Column<int>(type: "int", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_user", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ux_platform_user_email",
                schema: "postgres",
                table: "platform_user",
                column: "email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "platform_user",
                schema: "postgres");
        }
    }
}
