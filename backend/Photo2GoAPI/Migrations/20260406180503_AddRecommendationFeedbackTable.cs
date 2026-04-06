using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photo2GoAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRecommendationFeedbackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "GeneratedRoutes" (
                    "id" INTEGER NOT NULL CONSTRAINT "PK_GeneratedRoutes" PRIMARY KEY AUTOINCREMENT,
                    "user_id" INTEGER NOT NULL,
                    "created_at_utc" TEXT NOT NULL,
                    "file_json" TEXT NOT NULL,
                    "analysis_json" TEXT NOT NULL,
                    "similar_locations_json" TEXT NOT NULL,
                    CONSTRAINT "FK_GeneratedRoutes_Users_user_id" FOREIGN KEY ("user_id") REFERENCES "Users" ("id") ON DELETE CASCADE
                );
                """);

            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS "IX_GeneratedRoutes_user_id"
                ON "GeneratedRoutes" ("user_id");
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "Routes" (
                    "id" INTEGER NOT NULL CONSTRAINT "PK_Routes" PRIMARY KEY AUTOINCREMENT,
                    "user_id" INTEGER NOT NULL,
                    "name" TEXT NOT NULL,
                    "location_ids_json" TEXT NOT NULL,
                    "created_at_utc" TEXT NOT NULL
                );
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "RecommendationFeedback" (
                    "id" INTEGER NOT NULL CONSTRAINT "PK_RecommendationFeedback" PRIMARY KEY AUTOINCREMENT,
                    "DetectedLocationId" INTEGER NOT NULL,
                    "DetectedCategory" TEXT NOT NULL,
                    "DetectedObjectType" TEXT NOT NULL,
                    "Feedback" TEXT NOT NULL,
                    "UserId" INTEGER NULL,
                    "CreatedAtUtc" TEXT NOT NULL,
                    CONSTRAINT "FK_RecommendationFeedback_Locations_DetectedLocationId" FOREIGN KEY ("DetectedLocationId") REFERENCES "Locations" ("id") ON DELETE CASCADE,
                    CONSTRAINT "FK_RecommendationFeedback_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("id") ON DELETE SET NULL
                );
                """);

            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS "IX_RecommendationFeedback_DetectedLocationId"
                ON "RecommendationFeedback" ("DetectedLocationId");
                """);

            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS "IX_RecommendationFeedback_UserId"
                ON "RecommendationFeedback" ("UserId");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneratedRoutes");

            migrationBuilder.DropTable(
                name: "RecommendationFeedback");

            migrationBuilder.DropTable(
                name: "Routes");
        }
    }
}
