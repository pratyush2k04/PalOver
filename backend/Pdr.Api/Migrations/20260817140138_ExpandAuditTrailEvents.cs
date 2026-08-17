using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class ExpandAuditTrailEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "audit_actions",
                columns: new[] { "Id", "Action", "Details" },
                values: new object[,]
                {
                    { 5, "DocumentVersioned", "Document {documentName} was versioned to {version} by {actorName}" },
                    { 6, "DocumentUploaded", "Document {documentName} uploaded under category {categoryName} by {actorName}" },
                    { 7, "UnauthorizedAccess", "Unauthorized access attempt for email {email}" },
                    { 8, "AccountLockout", "Account locked out for user {userName}" },
                    { 9, "ClientContactDetailsChanged", "Contact details changed for client {clientName} by {actorName}" },
                    { 10, "ClientPersonaChanged", "Client persona changed for {clientName} by {actorName}" },
                    { 11, "LeadershipAllocated", "Leadership allocated for engagement {engagementName} by {actorName}" },
                    { 12, "UserRoleAllocated", "Role {roleName} allocated to user {targetUserName} by {actorName}" },
                    { 13, "PmAndTlAssigned", "PM and TL assigned for engagement {engagementName} by {actorName}" },
                    { 14, "PmAndTlReassigned", "PM and TL reassigned for engagement {engagementName} by {actorName}" },
                    { 15, "DayOneSummaryGenerated", "Day-One Summary generated for engagement {engagementName} by {actorName}" },
                    { 16, "MasterChecklistPublished", "Master checklist version {version} published by {actorName}" },
                    { 17, "BrownfieldItemAdded", "Brownfield finding {findingName} added to engagement {engagementName} by {actorName}" },
                    { 18, "DiscoveryItemAdded", "Discovery item {itemName} added to engagement {engagementName} by {actorName}" }
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

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 18);
        }
    }
}
