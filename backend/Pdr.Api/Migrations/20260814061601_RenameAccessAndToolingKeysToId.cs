using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameAccessAndToolingKeysToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnvironmentToolingItemId",
                table: "EnvironmentToolingItems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EnvironmentToolingDefinitionId",
                table: "EnvironmentToolingDefinitions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "AccessItemId",
                table: "AccessItems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "AccessItemDefinitionId",
                table: "AccessItemDefinitions",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EnvironmentToolingItems",
                newName: "EnvironmentToolingItemId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EnvironmentToolingDefinitions",
                newName: "EnvironmentToolingDefinitionId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AccessItems",
                newName: "AccessItemId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AccessItemDefinitions",
                newName: "AccessItemDefinitionId");
        }
    }
}
