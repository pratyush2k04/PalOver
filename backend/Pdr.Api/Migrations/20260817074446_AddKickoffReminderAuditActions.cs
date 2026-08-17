using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddKickoffReminderAuditActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "audit_actions",
                columns: new[] { "Id", "Action", "Details" },
                values: new object[,]
                {
                    { 5, "KickoffReminderScheduled", "Kickoff readiness reminder scheduled for engagement {engagementName} ({thresholdLabel} before kickoff)" },
                    { 6, "KickoffReminderFired", "A {thresholdLabel}-before-kickoff readiness reminder was sent for engagement {engagementName}" },
                    { 7, "KickoffReminderSuppressed", "Kickoff reminder for engagement {engagementName} was suppressed after readiness reached 100%" },
                    { 8, "KickoffReminderReactivated", "Kickoff reminder for engagement {engagementName} was reactivated after readiness dropped to {readinessScore}%" },
                    { 9, "KickoffReminderRetiredForBilling", "Kickoff reminders for engagement {engagementName} were retired after billing was signed off" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 9);
        }
    }
}
