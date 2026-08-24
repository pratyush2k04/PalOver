using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsPossibleAndAddCommitmentVarianceTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_possible",
                table: "realistic_commitments");

            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "link_pattern", "title", "updated_at", "updated_by" },
                values: new object[] { 18, "Commitment '{CommitmentTitle}' timeline variance flagged: {VarianceDescription}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, "CommitmentVarianceEscalated", "/engagements/{EngagementId}/workspace/handover", "Commitment Variance — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.AddColumn<bool>(
                name: "is_possible",
                table: "realistic_commitments",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }
    }
}
