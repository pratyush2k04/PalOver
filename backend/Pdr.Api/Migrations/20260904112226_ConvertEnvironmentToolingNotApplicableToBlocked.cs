using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConvertEnvironmentToolingNotApplicableToBlocked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotApplicableReason",
                table: "EnvironmentToolingItems",
                newName: "BlockReason");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BlockReason",
                table: "EnvironmentToolingItems",
                newName: "NotApplicableReason");
        }
    }
}
