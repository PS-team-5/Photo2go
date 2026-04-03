using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Photo2GoAPI.Data;
using Photo2GoAPI.Model;
using Photo2GoAPI.Models;

namespace Photo2GoAPI.Services;

public class GeneratedRouteService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly AppDbContext _db;

    public GeneratedRouteService(AppDbContext db)
    {
        _db = db;
    }

    public Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken)
    {
        return _db.Users.AnyAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task SaveAsync(
        int userId,
        AnalyzeImageResponse file,
        ImageAnalysisResult analysis,
        IReadOnlyList<SimilarLocationResult> similarLocations,
        CancellationToken cancellationToken)
    {
        await EnsureStorageAsync(cancellationToken);

        var generatedRoute = new GeneratedRoute
        {
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow,
            FileJson = JsonSerializer.Serialize(file, SerializerOptions),
            AnalysisJson = JsonSerializer.Serialize(analysis, SerializerOptions),
            SimilarLocationsJson = JsonSerializer.Serialize(similarLocations, SerializerOptions)
        };

        _db.GeneratedRoutes.Add(generatedRoute);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GeneratedRouteResponse>> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        await EnsureStorageAsync(cancellationToken);

        var generatedRoutes = await _db.GeneratedRoutes
            .AsNoTracking()
            .Where(route => route.UserId == userId)
            .OrderByDescending(route => route.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return generatedRoutes.Select(MapResponse).ToList();
    }

    private static GeneratedRouteResponse MapResponse(GeneratedRoute route)
    {
        return new GeneratedRouteResponse
        {
            Id = route.Id,
            CreatedAtUtc = route.CreatedAtUtc,
            File = Deserialize<AnalyzeImageResponse>(route.FileJson),
            Analysis = Deserialize<ImageAnalysisResult>(route.AnalysisJson),
            SimilarLocations = Deserialize<List<SimilarLocationResult>>(route.SimilarLocationsJson)
        };
    }

    private static T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, SerializerOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name}.");
    }

    private async Task EnsureStorageAsync(CancellationToken cancellationToken)
    {
        await _db.Database.ExecuteSqlRawAsync(
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
            """,
            cancellationToken);

        await _db.Database.ExecuteSqlRawAsync(
            """
            CREATE INDEX IF NOT EXISTS "IX_GeneratedRoutes_user_id"
            ON "GeneratedRoutes" ("user_id");
            """,
            cancellationToken);
    }
}
