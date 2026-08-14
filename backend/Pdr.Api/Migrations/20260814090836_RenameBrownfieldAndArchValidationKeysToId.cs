using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameBrownfieldAndArchValidationKeysToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscoveryFeatureItemId",
                table: "discovery_feature_items",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BrownfieldFindingId",
                table: "brownfield_findings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ArchitectureValidationId",
                table: "architecture_validations",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "discovery_feature_items",
                newName: "DiscoveryFeatureItemId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "brownfield_findings",
                newName: "BrownfieldFindingId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "architecture_validations",
                newName: "ArchitectureValidationId");
        }
    }
}
