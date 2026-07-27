using TaskBoard.Api.Data;
using TaskBoard.Api.DTOs.Admin;
using TaskBoard.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TaskBoard.Api.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .Select(u => new UserDto
            {
                AdminId = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();
    }



// 🚀 Proje Metotları
    public async Task<List<ProjectDto>> GetAllProjectsAsync()
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(p => p.Owner)
            .Include(p => p.Columns)
                .ThenInclude(c => c.Tasks)
            .Include(p => p.Members)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                OwnerId = p.OwnerId,
                OwnerName = p.Owner != null ? p.Owner.FullName : "Bilinmiyor",
                OwnerEmail = p.Owner != null ? p.Owner.Email : "",
                ColumnCount = p.Columns.Count,
                TaskCount = p.Columns.SelectMany(c => c.Tasks).Count(),
                MemberCount = p.Members.Count
            })
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> DeleteProjectAsync(int projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
            return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }




}