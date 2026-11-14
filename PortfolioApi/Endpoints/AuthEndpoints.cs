using Microsoft.AspNetCore.Identity;
using PortfolioApi.Dtos;

namespace PortfolioApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", Register);
    }

    private static async Task<IResult> Register(RegisterRequest request, UserManager<IdentityUser> userManager)
    {
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return TypedResults.BadRequest(result.Errors.Select(e => e.Description));
        }

        await userManager.AddToRoleAsync(user, "Gast");
        return TypedResults.Ok();
    }
}