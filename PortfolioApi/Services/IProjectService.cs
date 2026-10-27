using PortfolioApi.Dtos;

namespace PortfolioApi.Services;

public interface IProjectService
{
    Task<List<ProjectResponse>> GetAllAsync();
    Task<ProjectResponse?> GetByIdAsync(int id);
}