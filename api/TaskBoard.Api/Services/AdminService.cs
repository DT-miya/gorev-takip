using TaskBoard.Api.Data;
using TaskBoard.Api.DTOs.Admin;
using TaskBoard.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Exceptions;

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
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();
    }


    public async Task<StatsDto> GetStatsAsync()
    {
        return new StatsDto
        {
            UserCount = await _context.Users.CountAsync(),
            ProjectCount = await _context.Projects.CountAsync(),
            TaskCount = await _context.Tasks.CountAsync()
        };
    }

    public async Task<List<UserDto>> UpdateUserRoleAsync(int userId, string newRole, int currentAdminId)
{
    if (newRole != "Admin" && newRole != "User")
        throw new ConflictException("Geçersiz rol");

    if (userId == currentAdminId)
        throw new ConflictException("Kendi rolünüzü değiştiremezsiniz");

    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    if (user == null)
        throw new NotFoundException("Kullanıcı bulunamadı");

    user.Role = newRole;
    await _context.SaveChangesAsync();

    return await GetAllUsersAsync();
}



// 🚀 Proje Metotları
    public async Task<List<ProjectDto>> GetAllProjectsAsync(int? limit = null)
    {
        var query = _context.Projects
            .AsNoTracking()
            .Include(p => p.Owner)
            .Include(p => p.Columns)
                .ThenInclude(c => c.Tasks)
            .Include(p => p.Members)
            .OrderByDescending(p => p.CreatedAt)
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
            });

        if (limit is > 0)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<PagedResponseDto<ProjectDto>> GetProjectsPageAsync(
        int page,
        int pageSize,
        string? search,
        int? minColumns,
        int? minTasks,
        int? minMembers,
        bool showArchived)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.Projects.AsNoTracking().Where(p => p.IsArchived == showArchived)
            .Include(p => p.Owner)
            .Include(p => p.Columns)
                .ThenInclude(c => c.Tasks)
            .Include(p => p.Members)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p => p.Name.Contains(term));
        }

        if (minColumns.HasValue)
        {
            query = query.Where(p => p.Columns.Count >= minColumns.Value);
        }

        if (minTasks.HasValue)
        {
            query = query.Where(p => p.Columns.SelectMany(c => c.Tasks).Count() >= minTasks.Value);
        }

        if (minMembers.HasValue)
        {
            query = query.Where(p => p.Members.Count >= minMembers.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
            .ToListAsync();

        return new PagedResponseDto<ProjectDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
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

    public async Task<bool> ArchiveProjectAsync(int projectId)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
            return false;

        project.IsArchived = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnarchiveProjectAsync(int projectId)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
            return false;

        project.IsArchived = false;
        await _context.SaveChangesAsync();
        return true;
    }





}
