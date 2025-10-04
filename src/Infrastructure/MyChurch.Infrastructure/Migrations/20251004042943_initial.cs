using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "postgres");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "achievements",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    icon_url = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    threshold = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "address",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    street = table.Column<string>(type: "varchar(200)", nullable: false),
                    city = table.Column<string>(type: "varchar(100)", nullable: false),
                    state = table.Column<string>(type: "varchar(100)", nullable: false),
                    zip_code = table.Column<string>(type: "varchar(20)", nullable: false),
                    country = table.Column<string>(type: "varchar(100)", nullable: false),
                    Complement = table.Column<string>(type: "text", nullable: false),
                    neighborhood = table.Column<string>(type: "varchar(100)", nullable: false),
                    number = table.Column<string>(type: "varchar(20)", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_address", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "daily_challenges",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    faith_points_awarded = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_challenges", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "faith_levels",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    points_required = table.Column<int>(type: "integer", nullable: false),
                    icon_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_faith_levels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "families",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "integer", nullable: false),
                    family_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_families", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "family_invitations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "integer", nullable: false),
                    inviter_member_id = table.Column<int>(type: "integer", nullable: false),
                    InvitedMemberId = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    responded_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_family_invitations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "group_resource",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    file_url = table.Column<string>(type: "text", nullable: false),
                    uploaded_by_member_id = table.Column<int>(type: "integer", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_resource", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hymns",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    chorus = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    lyrics_author = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    melody_author = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hymns", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "imported_hymns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false),
                    author = table.Column<string>(type: "text", nullable: true),
                    lyrics = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imported_hymns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "journeys",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    icon_url = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journeys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "member_achievements",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "integer", nullable: false),
                    achievement_id = table.Column<int>(type: "integer", nullable: false),
                    awarded_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_achievements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "member_bible_reading_assignment",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    bible_reading_plan_id = table.Column<int>(type: "int", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_bible_reading_assignment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "member_journey_assignments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "integer", nullable: false),
                    journey_id = table.Column<int>(type: "integer", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_journey_assignments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pastoral_alerts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "integer", nullable: false),
                    member_id = table.Column<int>(type: "integer", nullable: false),
                    leader_id = table.Column<int>(type: "integer", nullable: true),
                    message = table.Column<string>(type: "text", nullable: false),
                    source = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pastoral_alerts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plan",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(100)", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    max_members = table.Column<int>(type: "int", nullable: false),
                    max_events = table.Column<int>(type: "int", nullable: false),
                    max_storage_gb = table.Column<int>(type: "int", nullable: false),
                    branches = table.Column<int>(type: "int", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pre_launch_interests",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: true),
                    church_name = table.Column<string>(type: "text", nullable: true),
                    church_role = table.Column<string>(type: "text", nullable: true),
                    comments = table.Column<string>(type: "text", nullable: true),
                    register_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    confirmation_token = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pre_launch_interests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_action_history",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    action_type = table.Column<string>(type: "varchar(100)", nullable: false),
                    action_data = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_action_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "verses_of_the_day",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    verse_text = table.Column<string>(type: "text", nullable: false),
                    reference = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_verses_of_the_day", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "versions",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    abbreviation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    publisher = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    publication_year = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_versions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "visitor",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    asaas_customer_id = table.Column<string>(type: "text", nullable: true),
                    score = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: true),
                    last_visit_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    needs_follow_up = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visitor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "church",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(200)", nullable: false),
                    logo_file_name = table.Column<string>(type: "varchar(200)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    address_id = table.Column<int>(type: "int", nullable: false),
                    platform_fee = table.Column<decimal>(type: "numeric(5,4)", nullable: false, defaultValue: 0.05m),
                    phone = table.Column<string>(type: "varchar(20)", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true),
                    asaas_customer_id = table.Column<string>(type: "varchar(50)", nullable: true),
                    document = table.Column<string>(type: "varchar(20)", nullable: true),
                    onboarding_qrcode = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Denomination = table.Column<string>(type: "text", nullable: true),
                    CoverPhoto = table.Column<string>(type: "text", nullable: true),
                    AverageRating = table.Column<double>(type: "double precision", nullable: true),
                    TotalReviews = table.Column<int>(type: "integer", nullable: false),
                    TotalVisits = table.Column<int>(type: "integer", nullable: false),
                    HasParking = table.Column<bool>(type: "boolean", nullable: false),
                    IsAccessible = table.Column<bool>(type: "boolean", nullable: false),
                    HasLiveStream = table.Column<bool>(type: "boolean", nullable: false),
                    HasChildMinistry = table.Column<bool>(type: "boolean", nullable: false),
                    Languages = table.Column<string>(type: "text", nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_church", x => x.id);
                    table.ForeignKey(
                        name: "FK_church_address_address_id",
                        column: x => x.address_id,
                        principalSchema: "postgres",
                        principalTable: "address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "child",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    family_id = table.Column<int>(type: "integer", nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_child", x => x.id);
                    table.ForeignKey(
                        name: "FK_child_families_family_id",
                        column: x => x.family_id,
                        principalTable: "families",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "imported_hymn_stanzas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    imported_hymn_id = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imported_hymn_stanzas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_imported_hymn_stanzas_imported_hymns_imported_hymn_id",
                        column: x => x.imported_hymn_id,
                        principalTable: "imported_hymns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "journey_stages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    journey_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    faith_points_awarded = table.Column<int>(type: "integer", nullable: false),
                    RequiresLeaderVerification = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_stages", x => x.id);
                    table.ForeignKey(
                        name: "FK_journey_stages_journeys_journey_id",
                        column: x => x.journey_id,
                        principalTable: "journeys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "books",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    abbreviation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    testament = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    version_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_books", x => x.id);
                    table.ForeignKey(
                        name: "FK_books_versions_version_id",
                        column: x => x.version_id,
                        principalSchema: "postgres",
                        principalTable: "versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "visitor_status_history",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    visitor_id = table.Column<int>(type: "integer", nullable: false),
                    old_status = table.Column<int>(type: "integer", nullable: false),
                    new_status = table.Column<int>(type: "integer", nullable: false),
                    changed_by_member_id = table.Column<int>(type: "integer", nullable: true),
                    changed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visitor_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_visitor_status_history_visitor_visitor_id",
                        column: x => x.visitor_id,
                        principalSchema: "postgres",
                        principalTable: "visitor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asset",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(200)", nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    description = table.Column<string>(type: "text", nullable: false),
                    photo = table.Column<string>(type: "varchar(500)", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    identification_code = table.Column<string>(type: "varchar(100)", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    condition = table.Column<string>(type: "varchar(50)", nullable: false),
                    purchase_date = table.Column<DateTime>(type: "timestamp", nullable: true),
                    location = table.Column<string>(type: "varchar(200)", nullable: false),
                    responsible = table.Column<string>(type: "varchar(100)", nullable: false),
                    last_maintenance = table.Column<DateTime>(type: "timestamp", nullable: true),
                    next_maintenance = table.Column<DateTime>(type: "timestamp", nullable: true),
                    warranty_until = table.Column<DateTime>(type: "timestamp", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset", x => x.id);
                    table.ForeignKey(
                        name: "FK_asset_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "banking_info",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    bank_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    bank_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    agency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    account = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    account_digit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    account_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    holder_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    holder_document = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    pix_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    pix_key_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banking_info", x => x.id);
                    table.ForeignKey(
                        name: "FK_banking_info_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bible_reading_plan",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(100)", nullable: false),
                    description = table.Column<string>(type: "varchar(500)", nullable: false),
                    duration_in_days = table.Column<int>(type: "int", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bible_reading_plan", x => x.id);
                    table.ForeignKey(
                        name: "FK_bible_reading_plan_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaigns",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "int", maxLength: 300, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    goal_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    amount_raised = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    cover_image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaigns", x => x.id);
                    table.ForeignKey(
                        name: "FK_campaigns_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cash_flow_categories",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    church_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cash_flow_categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_cash_flow_categories_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "varchar(200)", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    finish_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    location = table.Column<string>(type: "varchar(200)", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    event_type = table.Column<int>(type: "integer", nullable: false),
                    requires_participant_list = table.Column<bool>(type: "boolean", nullable: false),
                    worship_service_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event", x => x.id);
                    table.ForeignKey(
                        name: "FK_event_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "member",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(200)", nullable: false),
                    email = table.Column<string>(type: "varchar(200)", nullable: true),
                    photo = table.Column<string>(type: "varchar(500)", nullable: true),
                    password_hash = table.Column<string>(type: "varchar(500)", nullable: true),
                    phone = table.Column<string>(type: "varchar(20)", nullable: true),
                    birth_date = table.Column<DateTime>(type: "date", nullable: false),
                    is_baptized = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    baptized_date = table.Column<DateTime>(type: "date", nullable: true),
                    is_tither = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    marital_status = table.Column<string>(type: "varchar(50)", nullable: true),
                    member_since = table.Column<DateTime>(type: "date", nullable: true),
                    ministry = table.Column<string>(type: "varchar(100)", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    notes = table.Column<string>(type: "varchar(1000)", nullable: true),
                    address_id = table.Column<int>(type: "int", nullable: true),
                    birth_city = table.Column<string>(type: "varchar(100)", nullable: true),
                    birth_state = table.Column<string>(type: "varchar(100)", nullable: true),
                    role = table.Column<int>(type: "int", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true),
                    pending_approval = table.Column<bool>(type: "boolean", nullable: false),
                    family_id = table.Column<int>(type: "integer", nullable: true),
                    TotalFaithPoints = table.Column<int>(type: "integer", nullable: false),
                    DevotionalStreak = table.Column<int>(type: "integer", nullable: false),
                    LastCompletedActivityDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FaithLevelId = table.Column<int>(type: "integer", nullable: true),
                    engagement_score = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_address_address_id",
                        column: x => x.address_id,
                        principalSchema: "postgres",
                        principalTable: "address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_member_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_member_faith_levels_FaithLevelId",
                        column: x => x.FaithLevelId,
                        principalTable: "faith_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_member_families_family_id",
                        column: x => x.family_id,
                        principalTable: "families",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "subscription",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true),
                    external_reference = table.Column<string>(type: "varchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription", x => x.id);
                    table.ForeignKey(
                        name: "FK_subscription_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_subscription_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "postgres",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transfer_history",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    requested_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transfer_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_transfer_history_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "member_journey_progresses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "integer", nullable: false),
                    journey_stage_id = table.Column<int>(type: "integer", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_journey_progresses", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_journey_progresses_journey_stages_journey_stage_id",
                        column: x => x.journey_stage_id,
                        principalTable: "journey_stages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chapters",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    book_id = table.Column<int>(type: "integer", nullable: false),
                    chapter_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chapters", x => x.id);
                    table.ForeignKey(
                        name: "FK_chapters_books_book_id",
                        column: x => x.book_id,
                        principalSchema: "postgres",
                        principalTable: "books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bible_reading_plan_stage",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bible_reading_plan_id = table.Column<int>(type: "int", nullable: false),
                    order = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "varchar(200)", nullable: false),
                    verse_references = table.Column<string>(type: "varchar(500)", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bible_reading_plan_stage", x => x.id);
                    table.ForeignKey(
                        name: "FK_bible_reading_plan_stage_bible_reading_plan_bible_reading_p~",
                        column: x => x.bible_reading_plan_id,
                        principalSchema: "postgres",
                        principalTable: "bible_reading_plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_notification",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_notification", x => x.id);
                    table.ForeignKey(
                        name: "FK_event_notification_event_event_id",
                        column: x => x.event_id,
                        principalSchema: "postgres",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_recurrence",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    recurrence_type = table.Column<int>(type: "int", nullable: false),
                    frequency = table.Column<int>(type: "int", nullable: false),
                    recurrence_end_date = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_recurrence", x => x.id);
                    table.ForeignKey(
                        name: "FK_event_recurrence_event_event_id",
                        column: x => x.event_id,
                        principalSchema: "postgres",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worship_services",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    theme = table.Column<string>(type: "text", nullable: true),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worship_services", x => x.id);
                    table.ForeignKey(
                        name: "FK_worship_services_event_event_id",
                        column: x => x.event_id,
                        principalSchema: "postgres",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminNotices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChurchId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminNotices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminNotices_church_ChurchId",
                        column: x => x.ChurchId,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdminNotices_member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "cash_flow_entries",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cash_flow_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_cash_flow_entries_cash_flow_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "postgres",
                        principalTable: "cash_flow_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cash_flow_entries_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cash_flow_entries_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "credit_card_info",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    last4digits = table.Column<string>(type: "varchar(4)", nullable: false),
                    card_brand = table.Column<string>(type: "varchar(50)", nullable: false),
                    card_hash = table.Column<string>(type: "varchar(500)", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credit_card_info", x => x.id);
                    table.ForeignKey(
                        name: "FK_credit_card_info_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "donation",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: true),
                    visitor_id = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    platform_fee = table.Column<decimal>(type: "numeric(10,2)", nullable: false, defaultValue: 0m),
                    IsTransferred = table.Column<bool>(type: "boolean", nullable: false),
                    TransferredAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    campaign_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donation", x => x.id);
                    table.ForeignKey(
                        name: "FK_donation_campaigns_campaign_id",
                        column: x => x.campaign_id,
                        principalTable: "campaigns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_donation_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_donation_visitor_visitor_id",
                        column: x => x.visitor_id,
                        principalSchema: "postgres",
                        principalTable: "visitor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "engagement_event",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    points = table.Column<int>(type: "int", nullable: false),
                    event_type = table.Column<int>(type: "int", nullable: false),
                    event_reference_id = table.Column<string>(type: "varchar(255)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engagement_event", x => x.id);
                    table.ForeignKey(
                        name: "FK_engagement_event_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_participants",
                schema: "postgres",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_participants", x => new { x.event_id, x.member_id });
                    table.ForeignKey(
                        name: "FK_event_participants_event_event_id",
                        column: x => x.event_id,
                        principalSchema: "postgres",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_participants_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "group",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    leader_id = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    accepts_new_members = table.Column<bool>(type: "boolean", nullable: false),
                    cover_image_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group", x => x.id);
                    table.ForeignKey(
                        name: "FK_group_member_leader_id",
                        column: x => x.leader_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "member_configurations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    preferred_bible_version_id = table.Column<int>(type: "integer", nullable: true),
                    theme_preference = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Light"),
                    font_size = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Medium"),
                    enable_notifications = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    last_updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_configurations", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_configurations_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_member_configurations_versions_preferred_bible_version_id",
                        column: x => x.preferred_bible_version_id,
                        principalSchema: "postgres",
                        principalTable: "versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "member_document",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    number = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_document", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_document_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "member_favorite_verses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    version_id = table.Column<int>(type: "integer", nullable: false),
                    book_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    chapter_number = table.Column<int>(type: "integer", nullable: false),
                    verse_number = table.Column<int>(type: "integer", nullable: false),
                    date_favorited = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_favorite_verses", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_favorite_verses_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "presentations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    church_id = table.Column<int>(type: "int", nullable: false),
                    admin_user_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    current_slide_index = table.Column<int>(type: "integer", nullable: false),
                    is_live = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_presentations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_presentations_church_church_id",
                        column: x => x.church_id,
                        principalSchema: "postgres",
                        principalTable: "church",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_presentations_member_admin_user_id",
                        column: x => x.admin_user_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "verses",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chapter_id = table.Column<int>(type: "integer", nullable: false),
                    verse_number = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_verses", x => x.id);
                    table.ForeignKey(
                        name: "FK_verses_chapters_chapter_id",
                        column: x => x.chapter_id,
                        principalSchema: "postgres",
                        principalTable: "chapters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "member_bible_reading_progress",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    bible_reading_plan_id = table.Column<int>(type: "int", nullable: false),
                    bible_reading_plan_stage_id = table.Column<int>(type: "int", nullable: false),
                    date_completed = table.Column<DateTime>(type: "timestamp", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_bible_reading_progress", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_bible_reading_progress_bible_reading_plan_bible_read~",
                        column: x => x.bible_reading_plan_id,
                        principalSchema: "postgres",
                        principalTable: "bible_reading_plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_member_bible_reading_progress_bible_reading_plan_stage_bibl~",
                        column: x => x.bible_reading_plan_stage_id,
                        principalSchema: "postgres",
                        principalTable: "bible_reading_plan_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_member_bible_reading_progress_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prayer_request",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worship_service_id = table.Column<int>(type: "integer", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    request = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prayer_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_prayer_request_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_prayer_request_worship_services_worship_service_id",
                        column: x => x.worship_service_id,
                        principalSchema: "postgres",
                        principalTable: "worship_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worship_activities",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worship_service_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    order = table.Column<int>(type: "integer", nullable: false),
                    is_current = table.Column<bool>(type: "boolean", nullable: false),
                    donation_time = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worship_activities", x => x.id);
                    table.ForeignKey(
                        name: "FK_worship_activities_worship_services_worship_service_id",
                        column: x => x.worship_service_id,
                        principalSchema: "postgres",
                        principalTable: "worship_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worship_presences",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worship_service_id = table.Column<int>(type: "integer", nullable: false),
                    member_id = table.Column<int>(type: "integer", nullable: true),
                    visitor_id = table.Column<int>(type: "integer", nullable: true),
                    timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worship_presences", x => x.id);
                    table.ForeignKey(
                        name: "FK_worship_presences_worship_services_worship_service_id",
                        column: x => x.worship_service_id,
                        principalSchema: "postgres",
                        principalTable: "worship_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worship_schedule_items",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worship_service_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worship_schedule_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_worship_schedule_items_worship_services_worship_service_id",
                        column: x => x.worship_service_id,
                        principalSchema: "postgres",
                        principalTable: "worship_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "donation_worship_service",
                schema: "postgres",
                columns: table => new
                {
                    donation_id = table.Column<int>(type: "int", nullable: false),
                    worship_service_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donation_worship_service", x => new { x.donation_id, x.worship_service_id });
                    table.ForeignKey(
                        name: "FK_donation_worship_service_donation_donation_id",
                        column: x => x.donation_id,
                        principalSchema: "postgres",
                        principalTable: "donation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_donation_worship_service_worship_services_worship_service_id",
                        column: x => x.worship_service_id,
                        principalSchema: "postgres",
                        principalTable: "worship_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    payment_status = table.Column<string>(type: "varchar(50)", nullable: false),
                    transaction_id = table.Column<string>(type: "varchar(100)", nullable: false),
                    billing_type = table.Column<string>(type: "varchar(30)", nullable: true),
                    subscription_id = table.Column<int>(type: "int", nullable: true),
                    donation_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_donation_donation_id",
                        column: x => x.donation_id,
                        principalSchema: "postgres",
                        principalTable: "donation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_payment_subscription_subscription_id",
                        column: x => x.subscription_id,
                        principalSchema: "postgres",
                        principalTable: "subscription",
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

            migrationBuilder.CreateTable(
                name: "feed_post_images",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    feed_post_id = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feed_post_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_feed_post_images_feed_posts_feed_post_id",
                        column: x => x.feed_post_id,
                        principalSchema: "postgres",
                        principalTable: "feed_posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "child_group_assignments",
                columns: table => new
                {
                    child_id = table.Column<int>(type: "integer", nullable: false),
                    group_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_child_group_assignments", x => new { x.child_id, x.group_id });
                    table.ForeignKey(
                        name: "FK_child_group_assignments_child_child_id",
                        column: x => x.child_id,
                        principalTable: "child",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_child_group_assignments_group_group_id",
                        column: x => x.group_id,
                        principalTable: "group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_member",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    role_in_group = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    date_joined = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_member", x => x.id);
                    table.ForeignKey(
                        name: "FK_group_member_group_group_id",
                        column: x => x.group_id,
                        principalTable: "group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_member_member_member_id",
                        column: x => x.member_id,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "slides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    presentation_id = table.Column<int>(type: "integer", nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    content_type = table.Column<int>(type: "integer", nullable: false),
                    content_reference_json = table.Column<string>(type: "jsonb", nullable: false),
                    cached_display_text = table.Column<string>(type: "text", nullable: false),
                    cached_media_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_slides_presentations_presentation_id",
                        column: x => x.presentation_id,
                        principalTable: "presentations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worship_activity_bibles",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worship_activity_id = table.Column<int>(type: "integer", nullable: false),
                    bible_version_id = table.Column<int>(type: "integer", nullable: false),
                    book_id = table.Column<int>(type: "integer", nullable: false),
                    chapter_id = table.Column<int>(type: "integer", nullable: false),
                    verse_start = table.Column<int>(type: "integer", nullable: false),
                    verse_end = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worship_activity_bibles", x => x.id);
                    table.ForeignKey(
                        name: "FK_worship_activity_bibles_worship_activities_worship_activity~",
                        column: x => x.worship_activity_id,
                        principalSchema: "postgres",
                        principalTable: "worship_activities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worship_activity_hymns",
                schema: "postgres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    worship_activity_id = table.Column<int>(type: "integer", nullable: false),
                    hymn_id = table.Column<int>(type: "integer", nullable: false),
                    hymn_title = table.Column<string>(type: "text", nullable: true),
                    hymn_number = table.Column<string>(type: "text", nullable: true),
                    verse_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worship_activity_hymns", x => x.id);
                    table.ForeignKey(
                        name: "FK_worship_activity_hymns_worship_activities_worship_activity_~",
                        column: x => x.worship_activity_id,
                        principalSchema: "postgres",
                        principalTable: "worship_activities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    WorshipPresenceId = table.Column<int>(type: "integer", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_member_ReviewerId",
                        column: x => x.ReviewerId,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_worship_presences_WorshipPresenceId",
                        column: x => x.WorshipPresenceId,
                        principalSchema: "postgres",
                        principalTable: "worship_presences",
                        principalColumn: "id");
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

            migrationBuilder.CreateTable(
                name: "ReviewPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewId = table.Column<int>(type: "integer", nullable: false),
                    PhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    UploadedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewPhotos_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "ReviewVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewId = table.Column<int>(type: "integer", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    IsHelpful = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewVotes_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewVotes_member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "postgres",
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminNotices_ChurchId",
                table: "AdminNotices",
                column: "ChurchId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminNotices_MemberId",
                table: "AdminNotices",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_church_id",
                schema: "postgres",
                table: "asset",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_banking_info_church_id",
                schema: "postgres",
                table: "banking_info",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_bible_reading_plan_church_id",
                schema: "postgres",
                table: "bible_reading_plan",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_bible_reading_plan_stage_bible_reading_plan_id_order",
                schema: "postgres",
                table: "bible_reading_plan_stage",
                columns: new[] { "bible_reading_plan_id", "order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_books_version_id",
                schema: "postgres",
                table: "books",
                column: "version_id");

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_church_id",
                table: "campaigns",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_cash_flow_categories_church_id",
                schema: "postgres",
                table: "cash_flow_categories",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_cash_flow_entries_category_id",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_cash_flow_entries_church_id",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_cash_flow_entries_member_id",
                schema: "postgres",
                table: "cash_flow_entries",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_chapters_book_id",
                schema: "postgres",
                table: "chapters",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "IX_child_family_id",
                table: "child",
                column: "family_id");

            migrationBuilder.CreateIndex(
                name: "IX_child_group_assignments_group_id",
                table: "child_group_assignments",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_church_address_id",
                schema: "postgres",
                table: "church",
                column: "address_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_info_member_id",
                schema: "postgres",
                table: "credit_card_info",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_donation_campaign_id",
                schema: "postgres",
                table: "donation",
                column: "campaign_id");

            migrationBuilder.CreateIndex(
                name: "IX_donation_member_id",
                schema: "postgres",
                table: "donation",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_donation_visitor_id",
                schema: "postgres",
                table: "donation",
                column: "visitor_id");

            migrationBuilder.CreateIndex(
                name: "IX_donation_worship_service_worship_service_id",
                schema: "postgres",
                table: "donation_worship_service",
                column: "worship_service_id");

            migrationBuilder.CreateIndex(
                name: "IX_engagement_event_church_id",
                schema: "postgres",
                table: "engagement_event",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_engagement_event_member_id_created_at",
                schema: "postgres",
                table: "engagement_event",
                columns: new[] { "member_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_event_church_id",
                schema: "postgres",
                table: "event",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_notification_event_id",
                schema: "postgres",
                table: "event_notification",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_participants_member_id",
                schema: "postgres",
                table: "event_participants",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_recurrence_event_id",
                schema: "postgres",
                table: "event_recurrence",
                column: "event_id",
                unique: true);

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
                name: "IX_feed_post_images_feed_post_id",
                schema: "postgres",
                table: "feed_post_images",
                column: "feed_post_id");

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

            migrationBuilder.CreateIndex(
                name: "IX_group_leader_id",
                table: "group",
                column: "leader_id");

            migrationBuilder.CreateIndex(
                name: "IX_group_member_group_id_member_id",
                table: "group_member",
                columns: new[] { "group_id", "member_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_member_member_id",
                table: "group_member",
                column: "member_id");

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

            migrationBuilder.CreateIndex(
                name: "IX_hymn_verses_hymn_id",
                schema: "postgres",
                table: "hymn_verses",
                column: "hymn_id");

            migrationBuilder.CreateIndex(
                name: "IX_imported_hymn_stanzas_imported_hymn_id",
                table: "imported_hymn_stanzas",
                column: "imported_hymn_id");

            migrationBuilder.CreateIndex(
                name: "IX_journey_stages_journey_id",
                table: "journey_stages",
                column: "journey_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_address_id",
                schema: "postgres",
                table: "member",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_church_id",
                schema: "postgres",
                table: "member",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_FaithLevelId",
                schema: "postgres",
                table: "member",
                column: "FaithLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_member_family_id",
                schema: "postgres",
                table: "member",
                column: "family_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_bible_reading_assignment_member_id_bible_reading_pla~",
                schema: "postgres",
                table: "member_bible_reading_assignment",
                columns: new[] { "member_id", "bible_reading_plan_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_member_bible_reading_progress_bible_reading_plan_id",
                schema: "postgres",
                table: "member_bible_reading_progress",
                column: "bible_reading_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_bible_reading_progress_bible_reading_plan_stage_id",
                schema: "postgres",
                table: "member_bible_reading_progress",
                column: "bible_reading_plan_stage_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_bible_reading_progress_member_id_bible_reading_plan_~",
                schema: "postgres",
                table: "member_bible_reading_progress",
                columns: new[] { "member_id", "bible_reading_plan_stage_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_member_configurations_member_id_unique",
                table: "member_configurations",
                column: "member_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_member_configurations_preferred_bible_version_id",
                table: "member_configurations",
                column: "preferred_bible_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_document_member_id",
                schema: "postgres",
                table: "member_document",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "ix_member_favorite_verses_unique_verse",
                table: "member_favorite_verses",
                columns: new[] { "member_id", "version_id", "book_name", "chapter_number", "verse_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_member_journey_progresses_journey_stage_id",
                table: "member_journey_progresses",
                column: "journey_stage_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_donation_id",
                schema: "postgres",
                table: "payment",
                column: "donation_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_subscription_id",
                schema: "postgres",
                table: "payment",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_prayer_request_member_id",
                schema: "postgres",
                table: "prayer_request",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_prayer_request_worship_service_id",
                schema: "postgres",
                table: "prayer_request",
                column: "worship_service_id");

            migrationBuilder.CreateIndex(
                name: "IX_presentations_admin_user_id",
                table: "presentations",
                column: "admin_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_presentations_church_id",
                table: "presentations",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPhotos_ReviewId",
                table: "ReviewPhotos",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPhotos_UploadedAt",
                table: "ReviewPhotos",
                column: "UploadedAt");

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

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_WorshipPresenceId",
                table: "Reviews",
                column: "WorshipPresenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewVotes_MemberId",
                table: "ReviewVotes",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewVotes_ReviewId",
                table: "ReviewVotes",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewVotes_ReviewId_MemberId",
                table: "ReviewVotes",
                columns: new[] { "ReviewId", "MemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_slides_presentation_id",
                table: "slides",
                column: "presentation_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_church_id",
                schema: "postgres",
                table: "subscription",
                column: "church_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plan_id",
                schema: "postgres",
                table: "subscription",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_transfer_history_church_id",
                schema: "postgres",
                table: "transfer_history",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_verses_chapter_id",
                schema: "postgres",
                table: "verses",
                column: "chapter_id");

            migrationBuilder.CreateIndex(
                name: "IX_visitor_status_history_visitor_id",
                schema: "postgres",
                table: "visitor_status_history",
                column: "visitor_id");

            migrationBuilder.CreateIndex(
                name: "IX_worship_activities_worship_service_id",
                schema: "postgres",
                table: "worship_activities",
                column: "worship_service_id");

            migrationBuilder.CreateIndex(
                name: "IX_worship_activity_bibles_worship_activity_id",
                schema: "postgres",
                table: "worship_activity_bibles",
                column: "worship_activity_id");

            migrationBuilder.CreateIndex(
                name: "IX_worship_activity_hymns_worship_activity_id",
                schema: "postgres",
                table: "worship_activity_hymns",
                column: "worship_activity_id");

            migrationBuilder.CreateIndex(
                name: "IX_worship_presences_worship_service_id",
                schema: "postgres",
                table: "worship_presences",
                column: "worship_service_id");

            migrationBuilder.CreateIndex(
                name: "IX_worship_schedule_items_worship_service_id",
                schema: "postgres",
                table: "worship_schedule_items",
                column: "worship_service_id");

            migrationBuilder.CreateIndex(
                name: "IX_worship_services_event_id",
                schema: "postgres",
                table: "worship_services",
                column: "event_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "achievements");

            migrationBuilder.DropTable(
                name: "AdminNotices");

            migrationBuilder.DropTable(
                name: "asset",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "banking_info",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "cash_flow_entries",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "child_group_assignments");

            migrationBuilder.DropTable(
                name: "credit_card_info",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "daily_challenges");

            migrationBuilder.DropTable(
                name: "donation_worship_service",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "engagement_event",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "event_notification",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "event_participants",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "event_recurrence",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "family_invitations");

            migrationBuilder.DropTable(
                name: "feed_likes",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "feed_post_images",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "group_member");

            migrationBuilder.DropTable(
                name: "group_resource");

            migrationBuilder.DropTable(
                name: "GroupMeetingAttendances");

            migrationBuilder.DropTable(
                name: "GroupMeetingMemberNotes");

            migrationBuilder.DropTable(
                name: "hymn_verses",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "imported_hymn_stanzas");

            migrationBuilder.DropTable(
                name: "member_achievements");

            migrationBuilder.DropTable(
                name: "member_bible_reading_assignment",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "member_bible_reading_progress",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "member_configurations");

            migrationBuilder.DropTable(
                name: "member_document",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "member_favorite_verses");

            migrationBuilder.DropTable(
                name: "member_journey_assignments");

            migrationBuilder.DropTable(
                name: "member_journey_progresses");

            migrationBuilder.DropTable(
                name: "pastoral_alerts");

            migrationBuilder.DropTable(
                name: "payment",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "prayer_request",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "pre_launch_interests",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "ReviewPhotos");

            migrationBuilder.DropTable(
                name: "ReviewResponses");

            migrationBuilder.DropTable(
                name: "ReviewVotes");

            migrationBuilder.DropTable(
                name: "slides");

            migrationBuilder.DropTable(
                name: "transfer_history",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "user_action_history",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "verses",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "verses_of_the_day",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "visitor_status_history",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "worship_activity_bibles",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "worship_activity_hymns",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "worship_schedule_items",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "cash_flow_categories",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "child");

            migrationBuilder.DropTable(
                name: "feed_posts",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "GroupMeetings");

            migrationBuilder.DropTable(
                name: "hymns",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "imported_hymns");

            migrationBuilder.DropTable(
                name: "bible_reading_plan_stage",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "journey_stages");

            migrationBuilder.DropTable(
                name: "donation",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "subscription",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "presentations");

            migrationBuilder.DropTable(
                name: "chapters",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "worship_activities",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "group");

            migrationBuilder.DropTable(
                name: "bible_reading_plan",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "journeys");

            migrationBuilder.DropTable(
                name: "campaigns");

            migrationBuilder.DropTable(
                name: "visitor",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "plan",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "worship_presences",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "books",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "member",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "worship_services",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "versions",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "faith_levels");

            migrationBuilder.DropTable(
                name: "families");

            migrationBuilder.DropTable(
                name: "event",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "church",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "address",
                schema: "postgres");
        }
    }
}
