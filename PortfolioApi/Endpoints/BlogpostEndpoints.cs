using PortfolioApi.Dtos;
using PortfolioApi.Services;

namespace PortfolioApi.Endpoints;

public static class BlogpostEndpoints
{
    public static void MapBlogpostEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/blogposts").WithTags("Blogposts");

        group.MapGet("/", GetAll);
        group.MapGet("/{id:int}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:int}", Update);
        group.MapDelete("/{id:int}", Delete);
    }

    private static async Task<IResult> GetAll(IBlogpostService service)
    {
        var blogposts = await service.GetAllAsync();
        return TypedResults.Ok(blogposts);
    }

    private static async Task<IResult> GetById(int id, IBlogpostService service)
    {
        var blogpost = await service.GetByIdAsync(id);
        if (blogpost is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(blogpost);
    }

    private static async Task<IResult> Create(CreateBlogpostRequest request, IBlogpostService service)
    {
        var blogpost = await service.CreateAsync(request);
        return TypedResults.Created($"/api/blogposts/{blogpost.Id}", blogpost);
    }

    private static async Task<IResult> Update(int id, UpdateBlogpostRequest request, IBlogpostService service)
    {
        var blogpost = await service.UpdateAsync(id, request);
        if (blogpost is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(blogpost);
    }

    private static async Task<IResult> Delete(int id, IBlogpostService service)
    {
        var deleted = await service.DeleteAsync(id);
        if (!deleted)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }
}