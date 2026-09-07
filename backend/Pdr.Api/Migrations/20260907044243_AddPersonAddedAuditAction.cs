using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonAddedAuditAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "audit_actions",
                columns: new[] { "Id", "Action", "Details" },
                values: new object[,]
                {
                    { 32, "PersonAdded", "Person {personName} with role {roleName} was added by {actorName}" },
                    { 33, "ClientContactTransitioned", "Client contact transitioned from {outgoingName} to {incomingName} by {actorName}" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 33);
        }
    }
}
