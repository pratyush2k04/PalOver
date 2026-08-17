using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleTransitionsPerEngagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_client_contact_transitions_engagement_id",
                table: "client_contact_transitions");

            migrationBuilder.CreateIndex(
                name: "IX_client_contact_transitions_engagement_id",
                table: "client_contact_transitions",
                column: "engagement_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_client_contact_transitions_engagement_id",
                table: "client_contact_transitions");

            migrationBuilder.CreateIndex(
                name: "IX_client_contact_transitions_engagement_id",
                table: "client_contact_transitions",
                column: "engagement_id",
                unique: true);
        }
    }
}
