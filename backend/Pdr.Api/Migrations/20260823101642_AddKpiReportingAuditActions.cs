using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddKpiReportingAuditActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "audit_actions",
                columns: new[] { "Id", "Action", "Details" },
                values: new object[,]
                {
                    { 26, "KickoffReadyMarked", "Engagement {engagementName} marked kickoff-ready by {actorName}" },
                    { 27, "KickoffReadyReverted", "Kickoff-ready status reverted for engagement {engagementName} by {actorName}" },
                    { 28, "AccessItemBlocked", "Access item '{itemName}' blocked on engagement {engagementName} by {actorName}" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_EngagementId_AuditActionId",
                table: "audit_logs",
                columns: new[] { "EngagementId", "AuditActionId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_audit_logs_EngagementId_AuditActionId",
                table: "audit_logs");

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 28);
        }
    }
}
