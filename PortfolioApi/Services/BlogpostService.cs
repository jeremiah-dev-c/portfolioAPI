using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Dtos;
using PortfolioApi.Models;

namespace PortfolioApi.Services;

public class BlogpostService : IBlogpostService
{
    private readonly PortfolioDbContext _context;

    public BlogpostService(PortfolioDbContext context)
    {
        _context = context;
    }

    public async Task<List<BlogpostResponse>> GetAllAsync()
    {
        var blogposts = await _context.Blogposts.ToListAsync();
        return blogposts.Select(ToResponse).ToList();
    }

    public async Task<BlogpostResponse?> GetByIdAsync(int id)
    {
        var blogpost = await _context.Blogposts.FindAsync(id);
        return blogpost is null ? null : ToResponse(blogpost);
    }

    public async Task<BlogpostResponse> CreateAsync(CreateBlogpostRequest request)
    {
        var blogpost = new Blogpost
        {
            Titel = request.Titel,
            Inhoud = request.Inhoud,
            Publicatiedatum = DateTime.UtcNow
        };

        _context.Blogposts.Add(blogpost);
        await _context.SaveChangesAsync();

        return ToResponse(blogpost);
    }

    public async Task<BlogpostResponse?> UpdateAsync(int id, UpdateBlogpostRequest request)
    {
        var blogpost = await _context.Blogposts.FindAsync(id);
        if (blogpost is null)
        {
            return null;
        }

        blogpost.Titel = request.Titel;
        blogpost.Inhoud = request.Inhoud;

        await _context.SaveChangesAsync();

        return ToResponse(blogpost);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var blogpost = await _context.Blogposts.FindAsync(id);
        if (blogpost is null)
        {
            return false;
        }

        _context.Blogposts.Remove(blogpost);
        await _context.SaveChangesAsync();

        return true;
    }

    private static BlogpostResponse ToResponse(Blogpost blogpost)
    {
        return new BlogpostResponse
        {
            Id = blogpost.Id,
            Titel = blogpost.Titel,
            Inhoud = blogpost.Inhoud,
            Publicatiedatum = blogpost.Publicatiedatum
        };
    }
}