using System.Net;
using Xunit;

namespace Photo2GoAPI.IntegrationTests;

// Integracinis testas: tikrina API filtravima nuo request iki database ir response
public class LocationsFilterTests : IClassFixture<CustomWebApplicationFactory>
{
    
    private readonly HttpClient _client;
    
    public LocationsFilterTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    
    [Theory]
    // 1 atvejis – egzistuojanti kategorija → yra rezultatai
    [InlineData("Church", HttpStatusCode.OK, true)]
    // 2 atvejis – neegzistuojanti kategorija → nera rezultatu
    [InlineData("Random", HttpStatusCode.OK, false)]
    public async Task FilterLocations_ReturnsExpectedResults(
        string objectType,
        HttpStatusCode expectedStatusCode,
        bool shouldContainResults)
    {
        // siunciam HTTP GET uzklausa i API endpointa su filtru
        var response = await _client.GetAsync($"/api/locations/filter?objectType={objectType}");
        
        var content = await response.Content.ReadAsStringAsync();

        // tikriname ar status kodas teisingas (pvz. 200 OK)
        Assert.Equal(expectedStatusCode, response.StatusCode);
        
        if (shouldContainResults)
        {
            Assert.Contains("Church", content);
        }
        else
        {
            Assert.DoesNotContain("Church", content);
        }
    }
}