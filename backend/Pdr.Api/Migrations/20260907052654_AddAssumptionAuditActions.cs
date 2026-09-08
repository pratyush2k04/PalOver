using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAssumptionAuditActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "audit_actions",
                columns: new[] { "Id", "Action", "Details" },
                values: new object[,]
                {
                    { 42, "AssumptionCreated", "Assumption {assumptionDescription} created for engagement {engagementName}" },
                    { 43, "AssumptionUpdated", "Assumption {assumptionDescription} updated for engagement {engagementName}" },
                    { 44, "AssumptionValidated", "Assumption {assumptionDescription} validated to status {assumptionStatus} for engagement {engagementName}" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 44);
        }
    }
}
