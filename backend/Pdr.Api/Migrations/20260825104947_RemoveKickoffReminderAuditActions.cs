using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveKickoffReminderAuditActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 23);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "audit_actions",
                columns: new[] { "Id", "Action", "Details" },
                values: new object[,]
                {
                    { 19, "KickoffReminderScheduled", "Kickoff readiness reminder scheduled for engagement {engagementName} ({thresholdLabel} before kickoff)" },
                    { 20, "KickoffReminderFired", "A {thresholdLabel}-before-kickoff readiness reminder was sent for engagement {engagementName}" },
                    { 21, "KickoffReminderSuppressed", "Kickoff reminders for engagement {engagementName} ({thresholdsLabel} before kickoff on {kickoffDate}) were suppressed after readiness reached 100%" },
                    { 22, "KickoffReminderReactivated", "Kickoff reminders for engagement {engagementName} ({thresholdsLabel} before kickoff on {kickoffDate}) were reactivated after readiness dropped to {readinessScore}%" },
                    { 23, "KickoffReminderRetiredForBilling", "Kickoff reminders for engagement {engagementName} were retired after billing was signed off" }
                });
        }
    }
}
