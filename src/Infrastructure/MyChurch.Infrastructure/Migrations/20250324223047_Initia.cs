using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    street = table.Column<string>(type: "varchar(200)", nullable: false),
                    city = table.Column<string>(type: "varchar(100)", nullable: false),
                    state = table.Column<string>(type: "varchar(100)", nullable: false),
                    zip_code = table.Column<string>(type: "varchar(20)", nullable: false),
                    country = table.Column<string>(type: "varchar(100)", nullable: false),
                    neighborhood = table.Column<string>(type: "varchar(100)", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Plan",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(100)", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    max_members = table.Column<int>(type: "int", nullable: false),
                    max_events = table.Column<int>(type: "int", nullable: false),
                    max_storage_gb = table.Column<int>(type: "int", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Church",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(200)", nullable: false),
                    logo_file_name = table.Column<string>(type: "varchar(200)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    address_id = table.Column<int>(type: "int", nullable: false),
                    phone = table.Column<string>(type: "varchar(20)", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SubscriptionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Church", x => x.id);
                    table.ForeignKey(
                        name: "FK_Church_Address_address_id",
                        column: x => x.address_id,
                        principalTable: "Address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "varchar(200)", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.id);
                    table.ForeignKey(
                        name: "FK_Event_Church_church_id",
                        column: x => x.church_id,
                        principalTable: "Church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(200)", nullable: false),
                    email = table.Column<string>(type: "varchar(200)", nullable: false),
                    document = table.Column<string>(type: "varchar(50)", nullable: false),
                    photo = table.Column<string>(type: "varchar(500)", nullable: true),
                    password_hash = table.Column<string>(type: "varchar(500)", nullable: false),
                    phone = table.Column<string>(type: "varchar(20)", nullable: false),
                    birth_date = table.Column<DateTime>(type: "date", nullable: false),
                    is_baptized = table.Column<bool>(type: "boolean", nullable: false),
                    baptized_date = table.Column<DateTime>(type: "date", nullable: true),
                    is_tither = table.Column<bool>(type: "boolean", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    role = table.Column<int>(type: "int", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.id);
                    table.ForeignKey(
                        name: "FK_Member_Church_church_id",
                        column: x => x.church_id,
                        principalTable: "Church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.id);
                    table.ForeignKey(
                        name: "FK_Subscription_Church_church_id",
                        column: x => x.church_id,
                        principalTable: "Church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscription_Plan_plan_id",
                        column: x => x.plan_id,
                        principalTable: "Plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Donation",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donation", x => x.id);
                    table.ForeignKey(
                        name: "FK_Donation_Member_member_id",
                        column: x => x.member_id,
                        principalTable: "Member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_participants",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_participants", x => new { x.event_id, x.member_id });
                    table.ForeignKey(
                        name: "FK_event_participants_Event_event_id",
                        column: x => x.event_id,
                        principalTable: "Event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_participants_Member_member_id",
                        column: x => x.member_id,
                        principalTable: "Member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    subscription_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    payment_status = table.Column<string>(type: "varchar(50)", nullable: false),
                    transaction_id = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.id);
                    table.ForeignKey(
                        name: "FK_Payment_Subscription_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "Subscription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Church_address_id",
                table: "Church",
                column: "address_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donation_member_id",
                table: "Donation",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_Event_church_id",
                table: "Event",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_participants_member_id",
                table: "event_participants",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_Member_church_id",
                table: "Member",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_subscription_id",
                table: "Payment",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_church_id",
                table: "Subscription",
                column: "church_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_plan_id",
                table: "Subscription",
                column: "plan_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Donation");

            migrationBuilder.DropTable(
                name: "event_participants");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropTable(
                name: "Church");

            migrationBuilder.DropTable(
                name: "Plan");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
