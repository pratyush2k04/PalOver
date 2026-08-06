using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRiskRelationsToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessItemId",
                table: "Risks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnvironmentToolingItemId",
                table: "Risks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Risks_AccessItemId",
                table: "Risks",
                column: "AccessItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Risks_EnvironmentToolingItemId",
                table: "Risks",
                column: "EnvironmentToolingItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Risks_AccessItems_AccessItemId",
                table: "Risks",
                column: "AccessItemId",
                principalTable: "AccessItems",
                principalColumn: "AccessItemId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Risks_EnvironmentToolingItems_EnvironmentToolingItemId",
                table: "Risks",
                column: "EnvironmentToolingItemId",
                principalTable: "EnvironmentToolingItems",
                principalColumn: "EnvironmentToolingItemId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Risks_AccessItems_AccessItemId",
                table: "Risks");

            migrationBuilder.DropForeignKey(
                name: "FK_Risks_EnvironmentToolingItems_EnvironmentToolingItemId",
                table: "Risks");

            migrationBuilder.DropIndex(
                name: "IX_Risks_AccessItemId",
                table: "Risks");

            migrationBuilder.DropIndex(
                name: "IX_Risks_EnvironmentToolingItemId",
                table: "Risks");

            migrationBuilder.DropColumn(
                name: "AccessItemId",
                table: "Risks");

            migrationBuilder.DropColumn(
                name: "EnvironmentToolingItemId",
                table: "Risks");
        }
    }
}
