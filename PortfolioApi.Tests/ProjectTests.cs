using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PortfolioApi.Tests;

public class ProjectTests : IDisposable
{
    private readonly PortfolioApiFactory _factory;
    private readonly HttpClient _client;
    
    public ProjectTests()
    {
        _factory = new PortfolioApiFactory();
        _client = _factory.CreateClient();
    }
    
    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task GetAll_ZonderLogin_Geeft200()
    {

        var response = await _client.GetAsync("/api/projects");


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetAll_LegeDatabase_GeeftLegeLijst()
    {
        // Act
        var response = await _client.GetAsync("/api/projects");
        var projecten = await response.Content.ReadFromJsonAsync<List<JsonElement>>();

        // Assert
        Assert.NotNull(projecten);
        Assert.Empty(projecten);
    }

    [Fact]
    public async Task GetById_OnbekendId_Geeft404()
    {
        // Act
        var response = await _client.GetAsync("/api/projects/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}