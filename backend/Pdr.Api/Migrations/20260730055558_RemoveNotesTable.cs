using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.CreateTable(
                name: "EngagementStatuses",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StatusName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngagementStatuses", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "EngagementType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngagementType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Engagements",
                columns: table => new
                {
                    EngagementId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EngagementDescription = table.Column<string>(type: "text", nullable: true),
                    Nature = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EngagementTypeId = table.Column<int>(type: "integer", nullable: false),
                    KickoffDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BillingStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Engagements", x => x.EngagementId);
                    table.ForeignKey(
                        name: "FK_Engagements_EngagementStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EngagementStatuses",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Engagements_EngagementType_EngagementTypeId",
                        column: x => x.EngagementTypeId,
                        principalTable: "EngagementType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Engagements_EngagementTypeId",
                table: "Engagements",
                column: "EngagementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Engagements_StatusId",
                table: "Engagements",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Engagements");

            migrationBuilder.DropTable(
                name: "EngagementStatuses");

            migrationBuilder.DropTable(
                name: "EngagementType");

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                });
        }
    }
}
