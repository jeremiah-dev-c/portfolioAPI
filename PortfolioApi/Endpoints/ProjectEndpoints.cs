using PortfolioApi.Dtos;
using PortfolioApi.Services;

namespace PortfolioApi.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/projects").WithTags("Projects");

        group.MapGet("/", GetAll);
        group.MapGet("/{id:int}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:int}", Update);
        group.MapDelete("/{id:int}", Delete);
    }

    private static async Task<IResult> GetAll(IProjectService service)
    {
        var projects = await service.GetAllAsync();
        return TypedResults.Ok(projects);
    }

    private static async Task<IResult> GetById(int id, IProjectService service)
    {
        var project = await service.GetByIdAsync(id);
        if (project is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(project);
    }

    private static async Task<IResult> Create(CreateProjectRequest request, IProjectService service)
    {
        var project = await service.CreateAsync(request);
        return TypedResults.Created($"/api/projects/{project.Id}", project);
    }

    private static async Task<IResult> Update(int id, UpdateProjectRequest request, IProjectService service)
    {
        var project = await service.UpdateAsync(id, request);
        if (project is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(project);
    }

    private static async Task<IResult> Delete(int id, IProjectService service)
    {
        var deleted = await service.DeleteAsync(id);
        if (!deleted)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }
}