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
                    { 22, "DependencyCreated", "Dependency '{dependencyDescription}' created on engagement '{engagementName}' by {actorName}" },
                    { 23, "DependencyUpdated", "Dependency '{dependencyDescription}' updated on engagement '{engagementName}' by {actorName}" },
                    { 24, "DependencyDeleted", "Dependency '{dependencyDescription}' deleted on engagement '{engagementName}' by {actorName}" },
                    { 25, "DependencyEscalated", "Dependency '{dependencyDescription}' escalated on engagement '{engagementName}' by {actorName}" }
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
