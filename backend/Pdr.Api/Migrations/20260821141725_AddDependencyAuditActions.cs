using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDependencyAuditActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "audit_actions",
                columns: new[] { "Id", "Action", "Details" },
                values: new object[,]
                {
                    { 22, "DependencyCreated", "Dependency {dependencyDescription} ({dependencyType}) created for engagement {engagementName}" },
                    { 23, "DependencyUpdated", "Dependency {dependencyDescription} updated to status {dependencyStatus} for engagement {engagementName}" },
                    { 24, "DependencyDeleted", "Dependency {dependencyDescription} deleted from engagement {engagementName}" },
                    { 25, "DependencyEscalated", "Dependency {dependencyDescription} escalated to risk {riskTitle} for engagement {engagementName}" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 25);
        }
    }
}
