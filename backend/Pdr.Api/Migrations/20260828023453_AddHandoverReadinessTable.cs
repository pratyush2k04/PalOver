using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddHandoverReadinessTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HandoverReadinessDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandoverReadinessDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HandoverReadinessItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EngagementId = table.Column<int>(type: "integer", nullable: false),
                    Area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    BlockReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    HandoverReadinessDefinitionId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandoverReadinessItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HandoverReadinessItems_HandoverReadinessDefinitions_Handove~",
                        column: x => x.HandoverReadinessDefinitionId,
                        principalTable: "HandoverReadinessDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HandoverReadinessItems_engagements_EngagementId",
                        column: x => x.EngagementId,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HandoverReadinessItems_users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "HandoverReadinessDefinitions",
                columns: new[] { "Id", "Area", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "COMMERCIAL", 1, true, "Sign commercial contract" },
                    { 2, "COMMERCIAL", 2, true, "Confirm engagement budget" },
                    { 3, "COMMERCIAL", 3, true, "Confirm billing schedule" },
                    { 4, "HANDOVER", 4, true, "Complete sales-to-delivery handover call" },
                    { 5, "HANDOVER", 5, true, "Share CRM opportunity notes" },
                    { 6, "HANDOVER", 6, true, "Transfer key client contacts" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HandoverReadinessItems_EngagementId",
                table: "HandoverReadinessItems",
                column: "EngagementId");

            migrationBuilder.CreateIndex(
                name: "IX_HandoverReadinessItems_HandoverReadinessDefinitionId",
                table: "HandoverReadinessItems",
                column: "HandoverReadinessDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_HandoverReadinessItems_OwnerId",
                table: "HandoverReadinessItems",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HandoverReadinessItems");

            migrationBuilder.DropTable(
                name: "HandoverReadinessDefinitions");
        }
    }
}
