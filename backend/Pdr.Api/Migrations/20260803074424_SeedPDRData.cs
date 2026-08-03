using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedPDRData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "engagement_statuses",
                columns: new[] { "status_id", "status_name" },
                values: new object[,]
                {
                    { 1, "KickoffPending" },
                    { 2, "Active" },
                    { 3, "Inactive" }
                });

            migrationBuilder.InsertData(
                table: "engagement_types",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Greenfield" },
                    { 2, "Brownfield" }
                });

            migrationBuilder.InsertData(
                table: "engagements",
                columns: new[] { "engagement_id", "billing_start_date", "client_name", "created_at", "created_by", "engagement_description", "engagement_type_id", "kickoff_date", "status_id", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1L, null, "FinTech Solutions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Core Banking Greenfield Implementation", 1, new DateOnly(2026, 8, 15), 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2L, new DateOnly(2026, 7, 1), "HealthCare Plus", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Legacy System Upgrade and Support", 2, new DateOnly(2026, 6, 1), 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_assignments",
                columns: new[] { "engagement_id", "user_id" },
                values: new object[,]
                {
                    { 1L, 1 },
                    { 1L, 2 },
                    { 2L, 2 },
                    { 2L, 3 }
                });

            migrationBuilder.InsertData(
                table: "handover_info",
                columns: new[] { "handover_id", "business_problem", "client_background", "client_sentiment", "created_at", "created_by", "deal_origin", "engagement_id", "status_id", "updated_at", "updated_by" },
                values: new object[] { 1L, "They need a modern core banking platform to support new digital initiatives and mobile apps.", "FinTech Solutions is a mid-sized financial institution expanding its digital footprint.", "Excited but cautious about delivery timelines due to past vendor failures.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Strategic Partner Referral", 1L, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "commitments",
                columns: new[] { "commitment_id", "commitment_type_id", "created_at", "created_by", "description", "handover_id", "quantity", "updated_at", "updated_by" },
                values: new object[] { 1L, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Delivery of MVP banking module within 3 months of kickoff.", 1L, "1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commitments",
                keyColumn: "commitment_id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "engagement_assignments",
                keyColumns: new[] { "engagement_id", "user_id" },
                keyValues: new object[] { 1L, 1 });

            migrationBuilder.DeleteData(
                table: "engagement_assignments",
                keyColumns: new[] { "engagement_id", "user_id" },
                keyValues: new object[] { 1L, 2 });

            migrationBuilder.DeleteData(
                table: "engagement_assignments",
                keyColumns: new[] { "engagement_id", "user_id" },
                keyValues: new object[] { 2L, 2 });

            migrationBuilder.DeleteData(
                table: "engagement_assignments",
                keyColumns: new[] { "engagement_id", "user_id" },
                keyValues: new object[] { 2L, 3 });

            migrationBuilder.DeleteData(
                table: "engagement_statuses",
                keyColumn: "status_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "handover_info",
                keyColumn: "handover_id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "engagement_statuses",
                keyColumn: "status_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "engagement_types",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "engagement_statuses",
                keyColumn: "status_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "engagement_types",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
