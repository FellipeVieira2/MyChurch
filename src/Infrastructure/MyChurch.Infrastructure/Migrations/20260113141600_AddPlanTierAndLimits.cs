using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanTierAndLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "can_export_csv",
                schema: "postgres",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_export_pdf",
                schema: "postgres",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "has_advanced_permissions",
                schema: "postgres",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "has_audit_trail",
                schema: "postgres",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "has_automations",
                schema: "postgres",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "has_department_reports",
                schema: "postgres",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "max_admins",
                schema: "postgres",
                table: "plan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "max_donations_per_month",
                schema: "postgres",
                table: "plan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "max_leaders",
                schema: "postgres",
                table: "plan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "tier",
                schema: "postgres",
                table: "plan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_plan_tier",
                schema: "postgres",
                table: "plan",
                column: "tier",
                unique: true);

            // Seed/Upsert dos planos padrão com IDs fixos (1..4)
            migrationBuilder.Sql(""""""
INSERT INTO postgres.plan (
    id, tier, name, price,
    max_members, max_events, max_storage_gb, branches,
    max_admins, max_leaders, max_donations_per_month,
    shows_ads,
    can_export_csv, can_export_pdf,
    has_department_reports, has_advanced_permissions, has_audit_trail, has_automations,
    can_promote_church, can_promote_events,
    has_advanced_analytics, has_priority_support, has_verified_badge,
    created, updated
)
VALUES
    (1, 1, 'Free',     0.00,  200,  50,  1, 1, 2, 10,  100,  true,  false, false, false, false, false, false, false, false, false, false, false, NOW(), NULL),
    (2, 2, 'ProSmall', 19.90, 500, 200,  5, 1, 5, 25,  500,  false, true,  false, true,  false, false, false, false, false, false, false, false, NOW(), NULL),
    (3, 3, 'Pro',      39.90, 2000,500, 10, 2, 10,50, 2000,  false, true,  true,  true,  true,  false, false, false, false, true,  true,  false, NOW(), NULL),
    (4, 4, 'ProPlus',  79.90,10000,2000,50, 5, 50,200,999999,false, true,  true,  true,  true,  true,  true,  false, false, true,  true,  true,  NOW(), NULL)
ON CONFLICT (id)
DO UPDATE SET
    tier = EXCLUDED.tier,
    name = EXCLUDED.name,
    price = EXCLUDED.price,
    max_members = EXCLUDED.max_members,
    max_events = EXCLUDED.max_events,
    max_storage_gb = EXCLUDED.max_storage_gb,
    branches = EXCLUDED.branches,
    max_admins = EXCLUDED.max_admins,
    max_leaders = EXCLUDED.max_leaders,
    max_donations_per_month = EXCLUDED.max_donations_per_month,
    shows_ads = EXCLUDED.shows_ads,
    can_export_csv = EXCLUDED.can_export_csv,
    can_export_pdf = EXCLUDED.can_export_pdf,
    has_department_reports = EXCLUDED.has_department_reports,
    has_advanced_permissions = EXCLUDED.has_advanced_permissions,
    has_audit_trail = EXCLUDED.has_audit_trail,
    has_automations = EXCLUDED.has_automations,
    can_promote_church = EXCLUDED.can_promote_church,
    can_promote_events = EXCLUDED.can_promote_events,
    has_advanced_analytics = EXCLUDED.has_advanced_analytics,
    has_priority_support = EXCLUDED.has_priority_support,
    has_verified_badge = EXCLUDED.has_verified_badge,
    updated = NOW();
"""""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove os planos seed (mantém se havia outros no banco)
            migrationBuilder.Sql("DELETE FROM postgres.plan WHERE id IN (1,2,3,4);");

            migrationBuilder.DropIndex(
                name: "IX_plan_tier",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "can_export_csv",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "can_export_pdf",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "has_advanced_permissions",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "has_audit_trail",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "has_automations",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "has_department_reports",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "max_admins",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "max_donations_per_month",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "max_leaders",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "tier",
                schema: "postgres",
                table: "plan");
        }
    }
}
