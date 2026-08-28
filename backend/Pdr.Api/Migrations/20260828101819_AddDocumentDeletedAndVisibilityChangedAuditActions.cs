using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentDeletedAndVisibilityChangedAuditActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "audit_actions",
                columns: new[] { "Id", "Action", "Details" },
                values: new object[,]
                {
                    { 30, "DocumentDeleted", "Document {documentName} removed by {actorName} ({reason})" },
                    { 31, "DocumentVisibilityChanged", "Document {documentName} visibility changed to {visibility} by {actorName}" }
                });

            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "link_pattern", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 471928, "{ActorName} removed {DocumentName} from {EngagementName}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, "DocumentDeleted", "/engagements/{EngagementId}/workspace/documents", "Document removed — {DocumentName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 583046, "{UploaderName} uploaded version {Version} of {DocumentName} to {EngagementName}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, "DocumentVersionUploaded", "/engagements/{EngagementId}/workspace/documents", "New version uploaded — {DocumentName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 692157, "{ActorName} changed {DocumentName}'s visibility to {Visibility} in {EngagementName}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, "DocumentVisibilityChanged", "/engagements/{EngagementId}/workspace/documents", "Document visibility changed — {DocumentName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 471928);

            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 583046);

            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 692157);
        }
    }
}
