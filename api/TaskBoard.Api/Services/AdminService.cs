using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.DTOs.Admin;
using TaskBoard.Api.Interfaces;
using TaskBoard.Api.Exceptions;
using TaskBoard.Api.Extensions;

namespace TaskBoard.Api.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;
    private readonly IActivityLogService _logService;

    public AdminService(AppDbContext context, IActivityLogService logService)
    {
        _context = context;
        _logService = logService;
    }

    // 1. Sayfalanmış ve Filtreli Kullanıcı Listesi
    public async Task<PagedResult<UserDto>> GetAllUsersAsync(UserFilterParametersDto parameters)
    {
        var query = _context.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var searchTerm = parameters.Search.Trim().ToLower();
            query = query.Where(u => u.Email.ToLower().Contains(searchTerm) || 
                                     u.FullName.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(u => u.Id)
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();

        return new PagedResult<UserDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
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

    // 2. Kullanıcı Rolü Güncelleme (DTO Tabanlı Log Çağrısı)
    public async Task<UserDto> UpdateUserRoleAsync(int userId, string newRole, int currentAdminId, string currentAdminName)
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

       
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };
    }

    // 3. Proje Metotları
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

    public Task<bool> DeleteProjectAsync(int projectId, int currentAdminId, string currentAdminName)
    {
        throw new NotImplementedException();
    }
}