using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Photo2GoAPI.Data;
using Photo2GoAPI.Model;
using Photo2GoAPI.Models;
using Photo2GoAPI.Services;
using Xunit;

namespace Photo2GoAPI.IntegrationTests;

public class GeneratedRoutesTests
{
    [Fact]
    public async Task SaveAsync_PersistsGeneratedRoute_AndGetByUserIdReturnsIt()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var user = new User
        {
            Username = "route-tester",
            Email = "route-tester@example.com",
            Password = "secret123"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var analysis = new ImageAnalysisResult
        {
            Name = "Vilnius Cathedral",
            ObjectType = "Church",
            ArchitectureStyle = "Classicism",
            Period = "18th century",
            City = "Vilnius",
            Confidence = 0.95m
        };

        var similarPlaceService = new SimilarPlaceAlgorithService(db);
        var similarLocations = await similarPlaceService.FindTopSimilarAsync(analysis);

        var service = new GeneratedRouteService(db);
        await service.SaveAsync(
            user.Id,
            new AnalyzeImageResponse
            {
                OriginalFileName = "landmark.jpg",
                MimeType = "image/jpeg",
                Size = 4
            },
            analysis,
            similarLocations,
            CancellationToken.None);

        var savedRoutes = await service.GetByUserIdAsync(user.Id, CancellationToken.None);

        var savedRoute = Assert.Single(savedRoutes);
        Assert.True(savedRoute.Id > 0);
        Assert.Equal("landmark.jpg", savedRoute.File.OriginalFileName);
        Assert.Equal("image/jpeg", savedRoute.File.MimeType);
        Assert.Equal("Vilnius Cathedral", savedRoute.Analysis.Name);
        Assert.Equal("Vilnius", savedRoute.Analysis.City);
        Assert.NotEmpty(savedRoute.SimilarLocations);
        Assert.Equal(similarLocations.Count, savedRoute.SimilarLocations.Count);
        Assert.Equal(similarLocations[0].Id, savedRoute.SimilarLocations[0].Id);

        Assert.Equal(1, await db.GeneratedRoutes.CountAsync());
    }
}
