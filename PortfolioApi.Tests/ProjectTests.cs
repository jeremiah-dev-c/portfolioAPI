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
    private static object GeldigProject() => new
    {
        titel = "Mijn portfolio",
        beschrijving = "Een API voor mijn projecten en blogposts",
        categorie = "Backend",
        gitHubUrl = "https://github.com/jeremiah-dev-c/portfolioAPI",
        datum = "2026-11-01T00:00:00"
    };

    [Fact]
    public async Task Post_ZonderLogin_Geeft401()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/projects", GeldigProject());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_AlsGast_Geeft403()
    {
        // Arrange
        var gastClient = await _factory.CreateClientAsRoleAsync("Gast");

        // Act
        var response = await gastClient.PostAsJsonAsync("/api/projects", GeldigProject());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Post_AlsEditor_Geeft201EnGeeftHetProjectTerug()
    {
        // Arrange
        var editorClient = await _factory.CreateClientAsRoleAsync("Editor");

        // Act
        var response = await editorClient.PostAsJsonAsync("/api/projects", GeldigProject());
        var project = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Mijn portfolio", project.GetProperty("titel").GetString());
    }

    [Fact]
    public async Task Post_AlsEditor_ProjectIsDaarnaOpTeHalen()
    {
        // Arrange
        var editorClient = await _factory.CreateClientAsRoleAsync("Editor");
        var postResponse = await editorClient.PostAsJsonAsync("/api/projects", GeldigProject());
        var gemaakt = await postResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = gemaakt.GetProperty("id").GetInt32();

        // Act
        var response = await _client.GetAsync($"/api/projects/{id}");
        var opgehaald = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Mijn portfolio", opgehaald.GetProperty("titel").GetString());
    }

    [Fact]
    public async Task Post_AlsAdmin_Geeft201()
    {
        // Arrange
        var adminClient = await _factory.CreateClientAsRoleAsync("Admin");

        // Act
        var response = await adminClient.PostAsJsonAsync("/api/projects", GeldigProject());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_ZonderTitel_Geeft400()
    {
        // Arrange
        var editorClient = await _factory.CreateClientAsRoleAsync("Editor");
        var body = new
        {
            titel = "",
            beschrijving = "Een beschrijving",
            categorie = "Backend",
            gitHubUrl = "https://github.com/jeremiah-dev-c/portfolioAPI"
        };

        // Act
        var response = await editorClient.PostAsJsonAsync("/api/projects", body);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_OngeldigeGitHubUrl_Geeft400()
    {
        // Arrange
        var editorClient = await _factory.CreateClientAsRoleAsync("Editor");
        var body = new
        {
            titel = "Mijn portfolio",
            beschrijving = "Een beschrijving",
            categorie = "Backend",
            gitHubUrl = "geen-url"
        };

        // Act
        var response = await editorClient.PostAsJsonAsync("/api/projects", body);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}