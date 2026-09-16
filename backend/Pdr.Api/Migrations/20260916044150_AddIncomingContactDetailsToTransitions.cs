using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIncomingContactDetailsToTransitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "incoming_contact_background",
                table: "client_contact_transitions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "incoming_contact_email",
                table: "client_contact_transitions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "incoming_contact_sentiment",
                table: "client_contact_transitions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "incoming_contact_background",
                table: "client_contact_transitions");

            migrationBuilder.DropColumn(
                name: "incoming_contact_email",
                table: "client_contact_transitions");

            migrationBuilder.DropColumn(
                name: "incoming_contact_sentiment",
                table: "client_contact_transitions");
        }
    }
}
