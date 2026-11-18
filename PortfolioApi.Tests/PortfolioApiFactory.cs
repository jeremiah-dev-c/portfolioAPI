using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PortfolioApi.Data;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;

namespace PortfolioApi.Tests;

public class PortfolioApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public PortfolioApiFactory()
    {
        _connection.Open();

        var options = new DbContextOptionsBuilder<PortfolioDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new PortfolioDbContext(options);
        context.Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<PortfolioDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<PortfolioDbContext>>();

            services.AddDbContext<PortfolioDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }
    
    public async Task<HttpClient> CreateClientAsRoleAsync(string role)
    {
        var client = CreateClient();
        var email = $"{role.ToLower()}@test.nl";
        var password = "Test123!";

        await client.PostAsJsonAsync("/api/auth/register", new { email, password });

        if (role != "Gast")
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email);
            await userManager.AddToRoleAsync(user!, role);
        }

        await client.PostAsJsonAsync("/api/auth/login", new { email, password });

        return client;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}   