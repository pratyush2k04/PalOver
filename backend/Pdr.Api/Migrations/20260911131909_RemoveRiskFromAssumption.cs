using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRiskFromAssumption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assumptions_risks_LinkedRiskId",
                table: "Assumptions");

            migrationBuilder.DropIndex(
                name: "IX_Assumptions_LinkedRiskId",
                table: "Assumptions");

            migrationBuilder.DropColumn(
                name: "LinkedRiskId",
                table: "Assumptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LinkedRiskId",
                table: "Assumptions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assumptions_LinkedRiskId",
                table: "Assumptions",
                column: "LinkedRiskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assumptions_risks_LinkedRiskId",
                table: "Assumptions",
                column: "LinkedRiskId",
                principalTable: "risks",
                principalColumn: "id");
        }
    }
}
