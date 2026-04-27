using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Photo2GoAPI.Configuration;
using Photo2GoAPI.Data;
using Photo2GoAPI.Exceptions;
using Photo2GoAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Load local secrets from .env before binding options from configuration.
var envFileValues = EnvFileLoader.Load(Path.Combine(builder.Environment.ContentRootPath, ".env"));
builder.Configuration.AddInMemoryCollection(EnvFileLoader.ToConfigurationMap(envFileValues));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ImageUploadOptions>(
    builder.Configuration.GetSection(ImageUploadOptions.SectionName));
builder.Services.Configure<AiOptions>(
    builder.Configuration.GetSection(AiOptions.SectionName));
builder.Services.Configure<RouteGenerationOptions>(
    builder.Configuration.GetSection(RouteGenerationOptions.SectionName));
builder.Services.Configure<FormOptions>(options =>
{
    var imageUploadOptions = builder.Configuration
        .GetSection(ImageUploadOptions.SectionName)
        .Get<ImageUploadOptions>() ?? new ImageUploadOptions();

    options.MultipartBodyLengthLimit = imageUploadOptions.MaxFileSizeInBytes;
});
var sqliteConnectionString = SqliteDatabaseSetup.ResolveConnectionString(
    builder.Configuration,
    builder.Environment.ContentRootPath);
SqliteDatabaseSetup.EnsureLegacyLocationsBaseline(sqliteConnectionString);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(sqliteConnectionString));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<GeneratedRouteService>();
builder.Services.AddScoped<ImageUploadService>();
builder.Services.AddScoped<ImageAnalysisService>();
builder.Services.AddScoped<SimilarPlaceAlgorithService>();
builder.Services.AddScoped<RecommendationFeedbackStore>();
// Register the concrete AI provider behind an interface so it can be swapped later.
builder.Services.AddHttpClient<IImageAnalysisClient, OpenAiImageAnalysisClient>((serviceProvider, client) =>
{
    var aiOptions = serviceProvider.GetRequiredService<IOptions<AiOptions>>().Value;
    client.BaseAddress = new Uri(aiOptions.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(aiOptions.TimeoutSeconds);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }
    else
    {
        dbContext.Database.EnsureCreated();
    }
}

// Convert infrastructure and provider exceptions into stable API responses.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var imageUploadOptions = context.RequestServices
            .GetRequiredService<IOptions<ImageUploadOptions>>()
            .Value;
        var routeGenerationOptions = context.RequestServices
            .GetRequiredService<IOptions<RouteGenerationOptions>>()
            .Value;

        switch (exception)
        {
            case InvalidDataException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = $"Failas per didelis. Maksimalus leidziamas dydis yra {imageUploadOptions.MaxFileSizeInBytes} baitu.",
                    maxFileSizeInBytes = imageUploadOptions.MaxFileSizeInBytes
                });
                return;
            case ObjectNotRecognizedException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var objectNotRecognizedException = (ObjectNotRecognizedException)exception;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Objektas neatpazintas. Ikelkite kita nuotrauka.",
                    confidence = objectNotRecognizedException.Confidence,
                    minimumConfidence = objectNotRecognizedException.MinimumConfidence
                });
                return;
            case AiTimeoutException:
                context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "AI servisas per ilgai neatsake."
                });
                return;
            case RouteGenerationTimeoutException:
                context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = $"Nepavyko sugeneruoti marsruto per {routeGenerationOptions.TimeoutSeconds} sekundziu. Bandykite dar karta."
                });
                return;
            case AiResponseFormatException:
                context.Response.StatusCode = StatusCodes.Status502BadGateway;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "AI atsakymas buvo nepilnas arba netinkamo formato."
                });
                return;
            case AiUnavailableException:
                context.Response.StatusCode = StatusCodes.Status502BadGateway;
                var aiUnavailableException = (AiUnavailableException)exception;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Nepavyko atlikti AI analizes.",
                    providerStatusCode = app.Environment.IsDevelopment() ? aiUnavailableException.StatusCode : null,
                    providerResponse = app.Environment.IsDevelopment() ? aiUnavailableException.ProviderResponse : null
                });
                return;
            case AiIntegrationException:
                context.Response.StatusCode = StatusCodes.Status502BadGateway;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Nepavyko atlikti AI analizes.",
                    detail = app.Environment.IsDevelopment() ? exception.Message : null
                });
                return;
        }

        throw exception ?? new InvalidOperationException("Unexpected error.");
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program
{
}
