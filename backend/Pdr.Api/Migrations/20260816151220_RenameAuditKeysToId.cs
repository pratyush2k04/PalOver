using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameAuditKeysToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuditLogId",
                table: "audit_logs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "AuditActionId",
                table: "audit_actions",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "audit_logs",
                newName: "AuditLogId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "audit_actions",
                newName: "AuditActionId");
        }
    }
}
