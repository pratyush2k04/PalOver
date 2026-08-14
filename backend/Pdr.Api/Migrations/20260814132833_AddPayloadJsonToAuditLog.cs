using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPayloadJsonToAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PayloadJson",
                table: "audit_logs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "audit_actions",
                keyColumn: "AuditActionId",
                keyValue: 1,
                column: "Details",
                value: "User {userName} logged into the system");

            migrationBuilder.UpdateData(
                table: "audit_actions",
                keyColumn: "AuditActionId",
                keyValue: 2,
                column: "Details",
                value: "User {userName} was created by {actorName}");

            migrationBuilder.UpdateData(
                table: "audit_actions",
                keyColumn: "AuditActionId",
                keyValue: 3,
                column: "Details",
                value: "User {userName} was updated by {actorName}");

            migrationBuilder.UpdateData(
                table: "audit_actions",
                keyColumn: "AuditActionId",
                keyValue: 4,
                column: "Details",
                value: "User {userName} was deleted by {actorName}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayloadJson",
                table: "audit_logs");

            migrationBuilder.UpdateData(
                table: "audit_actions",
                keyColumn: "AuditActionId",
                keyValue: 1,
                column: "Details",
                value: "User logged into the system");

            migrationBuilder.UpdateData(
                table: "audit_actions",
                keyColumn: "AuditActionId",
                keyValue: 2,
                column: "Details",
                value: "A new user account was created");

            migrationBuilder.UpdateData(
                table: "audit_actions",
                keyColumn: "AuditActionId",
                keyValue: 3,
                column: "Details",
                value: "A user account was updated");

            migrationBuilder.UpdateData(
                table: "audit_actions",
                keyColumn: "AuditActionId",
                keyValue: 4,
                column: "Details",
                value: "A user account was deleted");
        }
    }
}
