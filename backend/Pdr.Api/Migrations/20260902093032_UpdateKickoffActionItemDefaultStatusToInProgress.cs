using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKickoffActionItemDefaultStatusToInProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "kickoff_action_items",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "InProgress",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Open");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "kickoff_action_items",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Open",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "InProgress");
        }
    }
}
