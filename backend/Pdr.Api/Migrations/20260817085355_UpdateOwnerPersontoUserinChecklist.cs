using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOwnerPersontoUserinChecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_engagement_checklist_items_people_owner_id",
                table: "engagement_checklist_items");

            migrationBuilder.AddForeignKey(
                name: "FK_engagement_checklist_items_users_owner_id",
                table: "engagement_checklist_items",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_engagement_checklist_items_users_owner_id",
                table: "engagement_checklist_items");

            migrationBuilder.AddForeignKey(
                name: "FK_engagement_checklist_items_people_owner_id",
                table: "engagement_checklist_items",
                column: "owner_id",
                principalTable: "people",
                principalColumn: "person_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
