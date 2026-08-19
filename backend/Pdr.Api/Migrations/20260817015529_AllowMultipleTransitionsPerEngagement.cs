using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleTransitionsPerEngagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_client_contact_transitions_engagement_id",
                table: "client_contact_transitions");

            migrationBuilder.CreateIndex(
                name: "IX_client_contact_transitions_engagement_id",
                table: "client_contact_transitions",
                column: "engagement_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM client_contact_transitions
                        GROUP BY engagement_id
                        HAVING COUNT(*) > 1
                    ) THEN
                        RAISE EXCEPTION
                            'Cannot rollback: multiple ClientContactTransition records exist for the same EngagementId.';
                    END IF;
                END
                $$;
                """);
            migrationBuilder.DropIndex(
                name: "IX_client_contact_transitions_engagement_id",
                table: "client_contact_transitions");

            migrationBuilder.CreateIndex(
                name: "IX_client_contact_transitions_engagement_id",
                table: "client_contact_transitions",
                column: "engagement_id",
                unique: true);
        }
    }
}
