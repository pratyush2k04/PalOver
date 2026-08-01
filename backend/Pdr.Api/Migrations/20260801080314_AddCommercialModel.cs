using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCommercialModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "commercial_model_id",
                table: "engagements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "commercial_models",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commercial_models", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "commercial_models",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Time and Material" },
                    { 2, "Fixed" },
                    { 3, "Capped T&M" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_engagements_commercial_model_id",
                table: "engagements",
                column: "commercial_model_id");

            migrationBuilder.AddForeignKey(
                name: "FK_engagements_commercial_models_commercial_model_id",
                table: "engagements",
                column: "commercial_model_id",
                principalTable: "commercial_models",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_engagements_commercial_models_commercial_model_id",
                table: "engagements");

            migrationBuilder.DropTable(
                name: "commercial_models");

            migrationBuilder.DropIndex(
                name: "IX_engagements_commercial_model_id",
                table: "engagements");

            migrationBuilder.DropColumn(
                name: "commercial_model_id",
                table: "engagements");
        }
    }
}
