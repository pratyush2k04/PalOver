using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AccessTracker_EnvironmentTooling_Risk_Models_Created : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessItemDefinitions",
                columns: table => new
                {
                    AccessItemDefinitionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsClientSide = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessItemDefinitions", x => x.AccessItemDefinitionId);
                });

            migrationBuilder.CreateTable(
                name: "EnvironmentToolingDefinitions",
                columns: table => new
                {
                    EnvironmentToolingDefinitionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentToolingDefinitions", x => x.EnvironmentToolingDefinitionId);
                });

            migrationBuilder.CreateTable(
                name: "Risks",
                columns: table => new
                {
                    RiskId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EngagementId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Risks", x => x.RiskId);
                    table.ForeignKey(
                        name: "FK_Risks_engagements_EngagementId",
                        column: x => x.EngagementId,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessItems",
                columns: table => new
                {
                    AccessItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EngagementId = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AccessItemDefinitionId = table.Column<int>(type: "integer", nullable: true),
                    IsClientSide = table.Column<bool>(type: "boolean", nullable: false),
                    TimeZoneNote = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GrantedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    BlockReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessItems", x => x.AccessItemId);
                    table.ForeignKey(
                        name: "FK_AccessItems_AccessItemDefinitions_AccessItemDefinitionId",
                        column: x => x.AccessItemDefinitionId,
                        principalTable: "AccessItemDefinitions",
                        principalColumn: "AccessItemDefinitionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccessItems_engagements_EngagementId",
                        column: x => x.EngagementId,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessItems_users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnvironmentToolingItems",
                columns: table => new
                {
                    EnvironmentToolingItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EnvironmentToolingDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    EngagementId = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    CustomName = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    NotApplicableReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BlockReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentToolingItems", x => x.EnvironmentToolingItemId);
                    table.ForeignKey(
                        name: "FK_EnvironmentToolingItems_EnvironmentToolingDefinitions_Envir~",
                        column: x => x.EnvironmentToolingDefinitionId,
                        principalTable: "EnvironmentToolingDefinitions",
                        principalColumn: "EnvironmentToolingDefinitionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EnvironmentToolingItems_engagements_EngagementId",
                        column: x => x.EngagementId,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnvironmentToolingItems_users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AccessItemDefinitions",
                columns: new[] { "AccessItemDefinitionId", "DisplayOrder", "IsActive", "IsClientSide", "Name" },
                values: new object[,]
                {
                    { 1, 1, true, false, "VPN Access" },
                    { 2, 2, true, true, "Jira" },
                    { 3, 3, true, true, "SandBox" }
                });

            migrationBuilder.InsertData(
                table: "EnvironmentToolingDefinitions",
                columns: new[] { "EnvironmentToolingDefinitionId", "DisplayOrder", "IsActive", "IsMandatory", "Name" },
                values: new object[,]
                {
                    { 1, 1, true, true, "CI/CD" },
                    { 2, 2, true, true, "Dev" },
                    { 3, 3, true, false, "UAT Environment" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessItems_AccessItemDefinitionId",
                table: "AccessItems",
                column: "AccessItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessItems_EngagementId",
                table: "AccessItems",
                column: "EngagementId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessItems_OwnerId",
                table: "AccessItems",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentToolingItems_EngagementId",
                table: "EnvironmentToolingItems",
                column: "EngagementId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentToolingItems_EnvironmentToolingDefinitionId",
                table: "EnvironmentToolingItems",
                column: "EnvironmentToolingDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentToolingItems_OwnerId",
                table: "EnvironmentToolingItems",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Risks_EngagementId",
                table: "Risks",
                column: "EngagementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessItems");

            migrationBuilder.DropTable(
                name: "EnvironmentToolingItems");

            migrationBuilder.DropTable(
                name: "Risks");

            migrationBuilder.DropTable(
                name: "AccessItemDefinitions");

            migrationBuilder.DropTable(
                name: "EnvironmentToolingDefinitions");
        }
    }
}
