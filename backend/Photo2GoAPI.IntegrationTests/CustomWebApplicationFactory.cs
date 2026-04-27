using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Photo2GoAPI.Data;
using Photo2GoAPI.Models;
using Photo2GoAPI.Services;

namespace Photo2GoAPI.IntegrationTests;

// Si klase paleidzia testine API versija.
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
 
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Naudojama Development aplinka, kad aplikacija veiktu kaip lokaliai kuriant projekta.
        builder.UseEnvironment("Development");

        // Cia pakeiciamos kai kurios originalios aplikacijos priklausomybes i testines.
        builder.ConfigureServices(services =>
        {
            // Pasalinamas tikras AppDbContext registravimas.
            services.RemoveAll<AppDbContext>();
            // Pasalinamos tikros AppDbContext parinktys.
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            // Pasalinamas tikras AI klientas, kuris kitaip bandytu kviesti isorini servisa.
            services.RemoveAll<IImageAnalysisClient>();

            // Pridedama paprasta testine DB atmintyje.
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("Photo2GoIntegrationTests"));
            // Vietoje tikro AI kliento pridedamas netikras, pilnai valdomas testuose.
            services.AddSingleton<IImageAnalysisClient, FakeImageAnalysisClient>();

            // Sukuriamas laikinas paslaugu scope, kad galetume paruosti testine DB.
            using var scope = services.BuildServiceProvider().CreateScope();
            // Pasiimamas DB kontekstas is testines aplikacijos.
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // Isvaloma sena testines DB busena.
            dbContext.Database.EnsureDeleted();
            // Sukuriama nauja testine DB su pradiniais duomenimis.
            dbContext.Database.EnsureCreated();
        });
    }

    // Netikras AI klientas, kuris vietoje tikro OpenAI grazina is anksto paruostus atsakymus.
    private sealed class FakeImageAnalysisClient : IImageAnalysisClient
    {
        // Sis metodas apsimeta, kad analizuoja nuotrauka.
        public async Task<AiImageAnalysisPayload> AnalyzeAsync(
            IFormFile image,
            CancellationToken cancellationToken = default)
        {
            // Failo vardas sumazinamas iki lowercase, kad butu paprasciau tikrinti salygas.
            var fileName = image.FileName.ToLowerInvariant();

            if (fileName.Contains("slow-timeout", StringComparison.Ordinal))
            {
                await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken);
            }

            // Jei failo vardas rodo low-confidence atveji, graziname silpna AI rezultata.
            if (fileName.Contains("low-confidence", StringComparison.Ordinal))
            {
                return new AiImageAnalysisPayload
                {
                    // Nurodomas netikro tiekejo pavadinimas.
                    Provider = "FakeAI",
                    // Grazinamas JSON su per mazu confidence, kad API sugeneruotu klaida.
                    JsonPayload = """
                                  {
                                    "name": "Unknown Monument",
                                    "objectType": "Church",
                                    "architectureStyle": "Classicism",
                                    "period": "18th century",
                                    "city": "Vilnius",
                                    "confidence": 0.65
                                  }
                                  """
                };
            }

            // Visais kitais atvejais grazinamas sekmingas AI rezultatas.
            return new AiImageAnalysisPayload
            {
                // Nurodomas netikro tiekejo pavadinimas.
                Provider = "FakeAI",
                // Grazinamas JSON su pakankamu confidence sekmingam scenarijui.
                JsonPayload = """
                              {
                                "name": "Vilnius Cathedral",
                                "objectType": "Church",
                                "architectureStyle": "Classicism",
                                "period": "18th century",
                                "city": "Vilnius",
                                "confidence": 0.95
                              }
                              """
            };
        }
    }
}
