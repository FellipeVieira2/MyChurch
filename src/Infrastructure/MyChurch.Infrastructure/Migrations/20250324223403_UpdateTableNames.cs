using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Church_Address_address_id",
                table: "Church");

            migrationBuilder.DropForeignKey(
                name: "FK_Donation_Member_member_id",
                table: "Donation");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Church_church_id",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_event_participants_Event_event_id",
                table: "event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_event_participants_Member_member_id",
                table: "event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_Member_Church_church_id",
                table: "Member");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Subscription_subscription_id",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Church_church_id",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Plan_plan_id",
                table: "Subscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plan",
                table: "Plan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Member",
                table: "Member");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Donation",
                table: "Donation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Church",
                table: "Church");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                table: "Address");

            migrationBuilder.EnsureSchema(
                name: "postgres");

            migrationBuilder.RenameTable(
                name: "Subscription",
                newName: "subscription",
                newSchema: "postgres");

            migrationBuilder.RenameTable(
                name: "Plan",
                newName: "plan",
                newSchema: "postgres");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "payment",
                newSchema: "postgres");

            migrationBuilder.RenameTable(
                name: "Member",
                newName: "member",
                newSchema: "postgres");

            migrationBuilder.RenameTable(
                name: "event_participants",
                newName: "event_participants",
                newSchema: "postgres");

            migrationBuilder.RenameTable(
                name: "Event",
                newName: "event",
                newSchema: "postgres");

            migrationBuilder.RenameTable(
                name: "Donation",
                newName: "donation",
                newSchema: "postgres");

            migrationBuilder.RenameTable(
                name: "Church",
                newName: "church",
                newSchema: "postgres");

            migrationBuilder.RenameTable(
                name: "Address",
                newName: "address",
                newSchema: "postgres");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_plan_id",
                schema: "postgres",
                table: "subscription",
                newName: "IX_subscription_plan_id");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_church_id",
                schema: "postgres",
                table: "subscription",
                newName: "IX_subscription_church_id");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_subscription_id",
                schema: "postgres",
                table: "payment",
                newName: "IX_payment_subscription_id");

            migrationBuilder.RenameIndex(
                name: "IX_Member_church_id",
                schema: "postgres",
                table: "member",
                newName: "IX_member_church_id");

            migrationBuilder.RenameIndex(
                name: "IX_Event_church_id",
                schema: "postgres",
                table: "event",
                newName: "IX_event_church_id");

            migrationBuilder.RenameIndex(
                name: "IX_Donation_member_id",
                schema: "postgres",
                table: "donation",
                newName: "IX_donation_member_id");

            migrationBuilder.RenameIndex(
                name: "IX_Church_address_id",
                schema: "postgres",
                table: "church",
                newName: "IX_church_address_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subscription",
                schema: "postgres",
                table: "subscription",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_plan",
                schema: "postgres",
                table: "plan",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payment",
                schema: "postgres",
                table: "payment",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_member",
                schema: "postgres",
                table: "member",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_event",
                schema: "postgres",
                table: "event",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_donation",
                schema: "postgres",
                table: "donation",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_church",
                schema: "postgres",
                table: "church",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_address",
                schema: "postgres",
                table: "address",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_church_address_address_id",
                schema: "postgres",
                table: "church",
                column: "address_id",
                principalSchema: "postgres",
                principalTable: "address",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_donation_member_member_id",
                schema: "postgres",
                table: "donation",
                column: "member_id",
                principalSchema: "postgres",
                principalTable: "member",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_church_church_id",
                schema: "postgres",
                table: "event",
                column: "church_id",
                principalSchema: "postgres",
                principalTable: "church",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_participants_event_event_id",
                schema: "postgres",
                table: "event_participants",
                column: "event_id",
                principalSchema: "postgres",
                principalTable: "event",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_participants_member_member_id",
                schema: "postgres",
                table: "event_participants",
                column: "member_id",
                principalSchema: "postgres",
                principalTable: "member",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_member_church_church_id",
                schema: "postgres",
                table: "member",
                column: "church_id",
                principalSchema: "postgres",
                principalTable: "church",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_subscription_subscription_id",
                schema: "postgres",
                table: "payment",
                column: "subscription_id",
                principalSchema: "postgres",
                principalTable: "subscription",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_subscription_church_church_id",
                schema: "postgres",
                table: "subscription",
                column: "church_id",
                principalSchema: "postgres",
                principalTable: "church",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_subscription_plan_plan_id",
                schema: "postgres",
                table: "subscription",
                column: "plan_id",
                principalSchema: "postgres",
                principalTable: "plan",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_church_address_address_id",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropForeignKey(
                name: "FK_donation_member_member_id",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropForeignKey(
                name: "FK_event_church_church_id",
                schema: "postgres",
                table: "event");

            migrationBuilder.DropForeignKey(
                name: "FK_event_participants_event_event_id",
                schema: "postgres",
                table: "event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_event_participants_member_member_id",
                schema: "postgres",
                table: "event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_member_church_church_id",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropForeignKey(
                name: "FK_payment_subscription_subscription_id",
                schema: "postgres",
                table: "payment");

            migrationBuilder.DropForeignKey(
                name: "FK_subscription_church_church_id",
                schema: "postgres",
                table: "subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_subscription_plan_plan_id",
                schema: "postgres",
                table: "subscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subscription",
                schema: "postgres",
                table: "subscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_plan",
                schema: "postgres",
                table: "plan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payment",
                schema: "postgres",
                table: "payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_member",
                schema: "postgres",
                table: "member");

            migrationBuilder.DropPrimaryKey(
                name: "PK_event",
                schema: "postgres",
                table: "event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_donation",
                schema: "postgres",
                table: "donation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_church",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropPrimaryKey(
                name: "PK_address",
                schema: "postgres",
                table: "address");

            migrationBuilder.RenameTable(
                name: "subscription",
                schema: "postgres",
                newName: "Subscription");

            migrationBuilder.RenameTable(
                name: "plan",
                schema: "postgres",
                newName: "Plan");

            migrationBuilder.RenameTable(
                name: "payment",
                schema: "postgres",
                newName: "Payment");

            migrationBuilder.RenameTable(
                name: "member",
                schema: "postgres",
                newName: "Member");

            migrationBuilder.RenameTable(
                name: "event_participants",
                schema: "postgres",
                newName: "event_participants");

            migrationBuilder.RenameTable(
                name: "event",
                schema: "postgres",
                newName: "Event");

            migrationBuilder.RenameTable(
                name: "donation",
                schema: "postgres",
                newName: "Donation");

            migrationBuilder.RenameTable(
                name: "church",
                schema: "postgres",
                newName: "Church");

            migrationBuilder.RenameTable(
                name: "address",
                schema: "postgres",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_subscription_plan_id",
                table: "Subscription",
                newName: "IX_Subscription_plan_id");

            migrationBuilder.RenameIndex(
                name: "IX_subscription_church_id",
                table: "Subscription",
                newName: "IX_Subscription_church_id");

            migrationBuilder.RenameIndex(
                name: "IX_payment_subscription_id",
                table: "Payment",
                newName: "IX_Payment_subscription_id");

            migrationBuilder.RenameIndex(
                name: "IX_member_church_id",
                table: "Member",
                newName: "IX_Member_church_id");

            migrationBuilder.RenameIndex(
                name: "IX_event_church_id",
                table: "Event",
                newName: "IX_Event_church_id");

            migrationBuilder.RenameIndex(
                name: "IX_donation_member_id",
                table: "Donation",
                newName: "IX_Donation_member_id");

            migrationBuilder.RenameIndex(
                name: "IX_church_address_id",
                table: "Church",
                newName: "IX_Church_address_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plan",
                table: "Plan",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Member",
                table: "Member",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event",
                table: "Event",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Donation",
                table: "Donation",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Church",
                table: "Church",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                table: "Address",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Church_Address_address_id",
                table: "Church",
                column: "address_id",
                principalTable: "Address",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Donation_Member_member_id",
                table: "Donation",
                column: "member_id",
                principalTable: "Member",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Church_church_id",
                table: "Event",
                column: "church_id",
                principalTable: "Church",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_participants_Event_event_id",
                table: "event_participants",
                column: "event_id",
                principalTable: "Event",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_participants_Member_member_id",
                table: "event_participants",
                column: "member_id",
                principalTable: "Member",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Member_Church_church_id",
                table: "Member",
                column: "church_id",
                principalTable: "Church",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Subscription_subscription_id",
                table: "Payment",
                column: "subscription_id",
                principalTable: "Subscription",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Church_church_id",
                table: "Subscription",
                column: "church_id",
                principalTable: "Church",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Plan_plan_id",
                table: "Subscription",
                column: "plan_id",
                principalTable: "Plan",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
