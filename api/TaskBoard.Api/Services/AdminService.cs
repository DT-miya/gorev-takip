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

        if (!string.IsNullOrWhiteSpace(parameters.SearchEmail))
        {

           
            var searchEmail = parameters.SearchEmail.Trim().ToLower();
            query = query.Where(u => u.Email.ToLower().Contains(searchEmail));
        }

        if (!string.IsNullOrWhiteSpace(parameters.SearchName))
        {
            var searchName = parameters.SearchName.Trim().ToLower();
            query = query.Where(u => u.FullName.ToLower().Contains(searchName));

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

    // 2. Kullanıcı Rolü Güncelleme
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
    public async Task<List<ProjectDto>> GetAllProjectsAsync(int? limit = null)
    {
        var query = _context.Projects
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                OwnerId = p.OwnerId,
                OwnerName = p.Owner != null ? (p.Owner.FullName ?? "İsimsiz") : "Bilinmiyor",
                OwnerEmail = p.Owner != null ? (p.Owner.Email ?? "") : "",
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

    // 🔍 ÇİFT FİLTRELİ SAYFALAMA METODU
    public async Task<PagedResponseDto<ProjectDto>> GetProjectsPageAsync(
        int page,
        int pageSize,
        string? projectName,
        string? ownerSearch,
        int? minColumns,
        int? minTasks,
        int? minMembers,
        bool showArchived)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.Projects
            .AsNoTracking()
            .Where(p => p.IsArchived == showArchived);

        if (!string.IsNullOrWhiteSpace(projectName))
        {
            var nameTerm = projectName.Trim();
            query = query.Where(p => EF.Functions.Like(p.Name, $"%{nameTerm}%"));
        }

        if (!string.IsNullOrWhiteSpace(ownerSearch))
        {
            var ownerTerm = ownerSearch.Trim();
            query = query.Where(p => 
                (p.Owner != null && p.Owner.FullName != null && EF.Functions.Like(p.Owner.FullName, $"%{ownerTerm}%")) ||
                (p.Owner != null && p.Owner.Email != null && EF.Functions.Like(p.Owner.Email, $"%{ownerTerm}%"))
            );
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
                OwnerName = p.Owner != null ? (p.Owner.FullName ?? "İsimsiz") : "Bilinmiyor",
                OwnerEmail = p.Owner != null ? (p.Owner.Email ?? "") : "",
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

    // ⚡ EKSİK OLAN PROJE GÖREVLERİ GETİRME METODU
    public async Task<object> GetProjectTasksAsync(int projectId)
    {
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.Column != null && t.Column.ProjectId == projectId)
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                t.DueDate,
                ColumnName = t.Column != null ? t.Column.Name : ""
            })
            .ToListAsync();

        return tasks;
    }

    // ⚡ EKSİK OLAN PROJE ÜYELERİ GETİRME METODU
    public async Task<object> GetProjectMembersAsync(int projectId)
    {
        var members = await _context.ProjectMembers
            .AsNoTracking()
            .Where(pm => pm.ProjectId == projectId)
            .Select(pm => new
            {
                pm.UserId,
                FullName = pm.User != null ? pm.User.FullName : "Bilinmeyen Kullanıcı",
                Email = pm.User != null ? pm.User.Email : "",
                pm.Role
            })
            .ToListAsync();

        return members;
    }

    public async Task<bool> DeleteProjectAsync(int projectId, int currentAdminId, string currentAdminName)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
            return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ArchiveProjectAsync(int projectId, int currentAdminId, string currentAdminName)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
            return false;

        project.IsArchived = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnarchiveProjectAsync(int projectId, int currentAdminId, string currentAdminName)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
            return false;

        project.IsArchived = false;
        await _context.SaveChangesAsync();
        return true;
    }
}