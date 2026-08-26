using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadershipAndPresalesNotificationsAndDropStaleColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockReason",
                table: "EnvironmentToolingItems");

            migrationBuilder.DropColumn(
                name: "ExpectedTurnaroundDays",
                table: "AccessItems");

            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "link_pattern", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 19, "You have been assigned as Engagement Leader for {EngagementName}. Please assign a Project Manager and Tech Lead to the engagement.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, "LeadershipActionRequired", "/engagements/{EngagementId}/workspace/overview", "Action Required: Assign PM & TL — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 20, "Presales added commitment '{CommitmentTitle}' for {EngagementName}. Please review and add realistic commitments.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, "PresalesCommitmentAdded", "/engagements/{EngagementId}/workspace/handover", "New Presales Commitment — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.AddColumn<string>(
                name: "BlockReason",
                table: "EnvironmentToolingItems",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpectedTurnaroundDays",
                table: "AccessItems",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AccessItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "ExpectedTurnaroundDays",
                value: null);

            migrationBuilder.UpdateData(
                table: "AccessItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "ExpectedTurnaroundDays",
                value: null);

            migrationBuilder.UpdateData(
                table: "AccessItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "ExpectedTurnaroundDays",
                value: null);

            migrationBuilder.UpdateData(
                table: "AccessItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "ExpectedTurnaroundDays",
                value: null);

            migrationBuilder.UpdateData(
                table: "AccessItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "ExpectedTurnaroundDays",
                value: null);

            migrationBuilder.UpdateData(
                table: "AccessItems",
                keyColumn: "Id",
                keyValue: 6,
                column: "ExpectedTurnaroundDays",
                value: null);

            migrationBuilder.UpdateData(
                table: "EnvironmentToolingItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "BlockReason",
                value: null);

            migrationBuilder.UpdateData(
                table: "EnvironmentToolingItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "BlockReason",
                value: null);

            migrationBuilder.UpdateData(
                table: "EnvironmentToolingItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "BlockReason",
                value: null);

            migrationBuilder.UpdateData(
                table: "EnvironmentToolingItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "BlockReason",
                value: null);

            migrationBuilder.UpdateData(
                table: "EnvironmentToolingItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "BlockReason",
                value: null);

            migrationBuilder.UpdateData(
                table: "EnvironmentToolingItems",
                keyColumn: "Id",
                keyValue: 6,
                column: "BlockReason",
                value: null);
        }
    }
}
