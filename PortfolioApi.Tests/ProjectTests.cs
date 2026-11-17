using System.Net;

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
}