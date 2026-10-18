using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddResourcingGapAuditAndNotificationTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "audit_actions",
                columns: new[] { "Id", "Action", "Details" },
                values: new object[,]
                {
                    { 19, "ResourcingGapFlagRaised", "Resourcing gap flag raised for engagement {engagementName}: {unconfirmedCount} request(s) open, {daysToBilling} day(s) until billing" },
                    { 20, "ResourcingGapFlagCleared", "Resourcing gap flag cleared for engagement {engagementName}: all requested resources confirmed" }
                });

            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "link_pattern", "title", "updated_at", "updated_by" },
                values: new object[] { 12, "{UnconfirmedCount} resource request(s) for {EngagementName} remain unconfirmed with {DaysToBilling} day(s) until billing starts ({BillingDate}).", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, "ResourcingGapFlagRaised", "/engagements/{EngagementId}/workspace/resources", "Resources Not Confirmed — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 12);
        }
    }
}
