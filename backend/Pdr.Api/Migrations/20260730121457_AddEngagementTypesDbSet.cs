using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEngagementTypesDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Engagements_EngagementType_EngagementTypeId",
                table: "Engagements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EngagementType",
                table: "EngagementType");

            migrationBuilder.RenameTable(
                name: "EngagementType",
                newName: "EngagementTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EngagementTypes",
                table: "EngagementTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Engagements_EngagementTypes_EngagementTypeId",
                table: "Engagements",
                column: "EngagementTypeId",
                principalTable: "EngagementTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Engagements_EngagementTypes_EngagementTypeId",
                table: "Engagements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EngagementTypes",
                table: "EngagementTypes");

            migrationBuilder.RenameTable(
                name: "EngagementTypes",
                newName: "EngagementType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EngagementType",
                table: "EngagementType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Engagements_EngagementType_EngagementTypeId",
                table: "Engagements",
                column: "EngagementTypeId",
                principalTable: "EngagementType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
