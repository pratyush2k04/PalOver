using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTemplateLinkPattern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "link_pattern",
                table: "notification_templates",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 1,
                column: "link_pattern",
                value: "/engagements/{EngagementId}/workspace/technical-readiness");

            migrationBuilder.UpdateData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 2,
                column: "link_pattern",
                value: "/engagements/{EngagementId}/workspace/overview");

            migrationBuilder.UpdateData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 3,
                column: "link_pattern",
                value: "/engagements/{EngagementId}/workspace/handover");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "link_pattern",
                table: "notification_templates");
        }
    }
}
