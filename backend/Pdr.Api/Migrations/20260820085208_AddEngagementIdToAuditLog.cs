using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEngagementIdToAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EngagementId",
                table: "audit_logs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_EngagementId",
                table: "audit_logs",
                column: "EngagementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_audit_logs_EngagementId",
                table: "audit_logs");

            migrationBuilder.DropColumn(
                name: "EngagementId",
                table: "audit_logs");
        }
    }
}
