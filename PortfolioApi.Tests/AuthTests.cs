using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace PortfolioApi.Tests;

public class AuthTests : IDisposable
{
    private const string Email = "test@test.nl";
    private const string Password = "Test123!";

    private readonly PortfolioApiFactory _factory;
    private readonly HttpClient _client;

    public AuthTests()
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
    public async Task Register_GeldigeGegevens_Geeft200()
    {
        // Arrange
        var body = new { email = Email, password = Password };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", body);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_GeldigeGegevens_GeeftGebruikerDeRolGast()
    {
        // Arrange
        var body = new { email = Email, password = Password };

        // Act
        await _client.PostAsJsonAsync("/api/auth/register", body);

        // Assert
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = await userManager.FindByEmailAsync(Email);
        Assert.NotNull(user);
        Assert.True(await userManager.IsInRoleAsync(user, "Gast"));
    }

    [Fact]
    public async Task Register_ZwakWachtwoord_Geeft400()
    {
        // Arrange
        var body = new { email = Email, password = "abc" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", body);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_OngeldigEmailadres_Geeft400()
    {
        // Arrange
        var body = new { email = "geen-email", password = Password };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", body);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_BestaandEmailadres_Geeft400()
    {
        // Arrange
        var body = new { email = Email, password = Password };
        await _client.PostAsJsonAsync("/api/auth/register", body);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", body);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_GoedWachtwoord_Geeft200EnZetCookie()
    {
        // Arrange
        var body = new { email = Email, password = Password };
        await _client.PostAsJsonAsync("/api/auth/register", body);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", body);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }

    [Fact]
    public async Task Login_FoutWachtwoord_Geeft401()
    {
        // Arrange
        await _client.PostAsJsonAsync("/api/auth/register", new { email = Email, password = Password });
        var foutBody = new { email = Email, password = "Fout123!" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", foutBody);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}