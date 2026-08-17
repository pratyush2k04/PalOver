using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKickoffReminderAuditActionDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 7,
                column: "Details",
                value: "Kickoff reminders for engagement {engagementName} ({thresholdsLabel} before kickoff on {kickoffDate}) were suppressed after readiness reached 100%");

            migrationBuilder.UpdateData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 8,
                column: "Details",
                value: "Kickoff reminders for engagement {engagementName} ({thresholdsLabel} before kickoff on {kickoffDate}) were reactivated after readiness dropped to {readinessScore}%");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 7,
                column: "Details",
                value: "Kickoff reminder for engagement {engagementName} was suppressed after readiness reached 100%");

            migrationBuilder.UpdateData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 8,
                column: "Details",
                value: "Kickoff reminder for engagement {engagementName} was reactivated after readiness dropped to {readinessScore}%");
        }
    }
}
