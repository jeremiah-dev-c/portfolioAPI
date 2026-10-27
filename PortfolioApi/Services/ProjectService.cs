using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Dtos;
using PortfolioApi.Models;

namespace PortfolioApi.Services;

public class ProjectService : IProjectService
{
    private readonly PortfolioDbContext _context;

    public ProjectService(PortfolioDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProjectResponse>> GetAllAsync()
    {
        var projects = await _context.Projects.ToListAsync();
        return projects.Select(ToResponse).ToList();
    }

    public async Task<ProjectResponse?> GetByIdAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        return project is null ? null : ToResponse(project);
    }

    private static ProjectResponse ToResponse(Project project)
    {
        return new ProjectResponse
        {
            Id = project.Id,
            Titel = project.Titel,
            Beschrijving = project.Beschrijving,
            Categorie = project.Categorie,
            GitHubUrl = project.GitHubUrl,
            Datum = project.Datum
        };
    }
}