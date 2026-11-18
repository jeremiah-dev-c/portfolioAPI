using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PortfolioApi.Tests;

public class BlogpostTests : IDisposable
{
    private readonly PortfolioApiFactory _factory;
    private readonly HttpClient _client;

    public BlogpostTests()
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
        // Act
        var response = await _client.GetAsync("/api/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_LegeDatabase_GeeftLegeLijst()
    {
        // Act
        var response = await _client.GetAsync("/api/blogposts");
        var blogposts = await response.Content.ReadFromJsonAsync<List<JsonElement>>();

        // Assert
        Assert.NotNull(blogposts);
        Assert.Empty(blogposts);
    }

    [Fact]
    public async Task GetById_OnbekendId_Geeft404()
    {
        // Act
        var response = await _client.GetAsync("/api/blogposts/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Post_ZonderLogin_Geeft401()
    {
        // Arrange
        var body = new { titel = "Mijn blog", inhoud = "Inhoud van de blog" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/blogposts", body);
    
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_AlsGast_Geeft403()
    {
        // Arrange
        var gastClient = await _factory.CreateClientAsRoleAsync("Gast");
        var body = new { titel = "Mijn blog", inhoud = "Inhoud van de blog" };
    
        // Act
        var response = await gastClient.PostAsJsonAsync("/api/blogposts", body);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Post_AlsEditor_Geeft201EnGeeftDeBlogpostTerug()
    {
        // Arrange
        var editorClient = await _factory.CreateClientAsRoleAsync("Editor"); 
        var body = new { titel = "Mijn blog", inhoud = "Inhoud van de blog" };

        // Act
        var response = await editorClient.PostAsJsonAsync("/api/blogposts", body);
        var blogpost = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Mijn blog", blogpost.GetProperty("titel").GetString());
    }

    [Fact]
    public async Task Post_AlsEditor_BlogpostIsDaarnaOpTeHalen()
    {
        // Arrange
        var editorClient = await _factory.CreateClientAsRoleAsync("Editor");
        var body = new { titel = "Mijn blog", inhoud = "Inhoud van de blog" };
        var postResponse = await editorClient.PostAsJsonAsync("/api/blogposts", body);
        var gemaakt = await postResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = gemaakt.GetProperty("id").GetInt32();

        // Act
        var response = await _client.GetAsync($"/api/blogposts/{id}");
        var opgehaald = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Mijn blog", opgehaald.GetProperty("titel").GetString());
    }

    [Fact]
    public async Task Post_AlsAdmin_Geeft201()
    {
        // Arrange
        var adminClient = await _factory.CreateClientAsRoleAsync("Admin");
        var body = new { titel = "Mijn blog", inhoud = "Inhoud van de blog" };

        // Act
        var response = await adminClient.PostAsJsonAsync("/api/blogposts", body);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}