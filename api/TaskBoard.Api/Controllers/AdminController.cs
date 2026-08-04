using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.DTOs.Admin;
using TaskBoard.Api.Extensions; // GetClientIpAddress eklentisi için
using TaskBoard.Api.Interfaces;
using TaskBoard.Api.Models;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ITaskService _taskService;
    private readonly IProjectService _projectService;
    private readonly AppDbContext _context;
    private readonly IActivityLogService _logService;

    public AdminController(
        IAdminService adminService,
        ITaskService taskService,
        IProjectService projectService,
        AppDbContext context,
        IActivityLogService logService)
    {
        _adminService = adminService;
        _taskService = taskService;
        _projectService = projectService;
        _context = context;
        _logService = logService;
    }

    private int GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(idClaim, out var userId) ? userId : 0;
    }

    private string GetAdminName()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.FindFirst(ClaimTypes.Email)?.Value 
            ?? "Admin";
    }

    // 1. Sayfalanmış ve Filtreli Kullanıcı Listesi
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterParametersDto parameters)
    {
        var result = await _adminService.GetAllUsersAsync(parameters);
        return Ok(result);
    }

    // 2. Sistem İstatistikleri
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await _adminService.GetStatsAsync();
        return Ok(result);
    }

    // 3. Kullanıcı Rolü Güncelleme
    [HttpPut("users/{userId:int}/role")]
    public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateRoleRequest dto)
    {
        var currentAdminId = GetUserId();
        var currentAdminName = GetAdminName();

        var result = await _adminService.UpdateUserRoleAsync(userId, dto.Role, currentAdminId, currentAdminName);

        string clientIp = HttpContext.GetClientIpAddress();

        // Activity Log Kaydı
        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = currentAdminId,
            UserMail = currentAdminName,
            Action = "ROLE_CHANGE",
            Description = $"{currentAdminName}, #{userId} Id'li kullanıcının rolünü '{dto.Role}' olarak değiştirdi.",
            IpAddress = clientIp
        });

        return Ok(result);
    }

    // 4. Tüm Projeler (Limitli / Basit Liste)
    [HttpGet("projects/all")]
    public async Task<IActionResult> GetAllProjects([FromQuery] int? limit = null)
    {
        var result = await _adminService.GetAllProjectsAsync(limit);
        return Ok(result);
    }

    // 5. Sayfalanmış Proje Listesi (Çift Filtreli)
    [HttpGet("projects")]
    public async Task<IActionResult> GetProjectsPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? projectName = null,
        [FromQuery] string? ownerSearch = null,
        [FromQuery] int? minColumns = null,
        [FromQuery] int? minTasks = null,
        [FromQuery] int? minMembers = null,
        [FromQuery] bool showArchived = false)
    {
        var result = await _adminService.GetProjectsPageAsync(
            page,
            pageSize,
            projectName,
            ownerSearch,
            minColumns,
            minTasks,
            minMembers,
            showArchived
        );

        return Ok(result);
    }

    // 6. Proje Görevlerini Getir
    [HttpGet("projects/{projectId:int}/tasks")]
    public async Task<IActionResult> GetProjectTasks(int projectId)
    {
        var userId = GetUserId();
        var board = await _taskService.GetFullBoardAsync(projectId, userId, skipmemberCheck: true);
        return Ok(board);
    }

    // 7. Proje Üyelerini Getir
    [HttpGet("projects/{projectId:int}/members")]
    public async Task<IActionResult> GetProjectMembers(int projectId)
    {
        var userId = GetUserId();
        var members = await _projectService.GetMembersAsync(projectId, userId, skipmemberCheck: true);
        return Ok(members);
    }

    // 8. Proje Silme
    [HttpDelete("projects/{projectId:int}")]
    public async Task<IActionResult> DeleteProject(int projectId)
    {
        var currentAdminId = GetUserId();
        var currentAdminName = GetAdminName();

        var success = await _adminService.DeleteProjectAsync(projectId, currentAdminId, currentAdminName);
        if (!success)
            return NotFound(new { message = "Proje bulunamadı." });

        string clientIp = HttpContext.GetClientIpAddress();

        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = currentAdminId,
            UserMail = currentAdminName,
            Action = "PROJECT_DELETE",
            Description = $"{currentAdminName}, #{projectId} Id'li projeyi sildi.",
            IpAddress = clientIp
        });

        return Ok(new { message = "Proje başarıyla silindi." });
    }

    // 9. Proje Arşivleme
    [HttpPut("projects/{projectId:int}/archive")]
    public async Task<IActionResult> ArchiveProject(int projectId)
    {
        var currentAdminId = GetUserId();
        var currentAdminName = GetAdminName();

        var success = await _adminService.ArchiveProjectAsync(projectId, currentAdminId, currentAdminName);
        if (!success)
            return NotFound(new { message = "Proje bulunamadı." });

        string clientIp = HttpContext.GetClientIpAddress();

        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = currentAdminId,
            UserMail = currentAdminName,
            Action = "PROJECT_ARCHIVE",
            Description = $"{currentAdminName}, #{projectId} Id'li projeyi arşivledi.",
            IpAddress = clientIp
        });

        return Ok(new { message = "Proje başarıyla arşivlendi." });
    }

    // 10. Proje Arşivden Çıkarma
    [HttpPut("projects/{projectId:int}/unarchive")]
    public async Task<IActionResult> UnarchiveProject(int projectId)
    {
        var currentAdminId = GetUserId();
        var currentAdminName = GetAdminName();

        var success = await _adminService.UnarchiveProjectAsync(projectId, currentAdminId, currentAdminName);
        if (!success)
            return NotFound(new { message = "Proje bulunamadı." });

        return Ok(new { message = "Proje geri getirildi." });
    }

    // 11. Sistem Loglarını Getir
    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs([FromQuery] LogFilterParametersDto parameters)
    {
        var query = _context.ActivityLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchUserMail))
        {
            var searchUserMail = parameters.SearchUserMail.Trim().ToLower();
            query = query.Where(l => l.UserMail.ToLower().Contains(searchUserMail));
        }
        
        if (!string.IsNullOrWhiteSpace(parameters.SearchAction))
        {
            var searchAction = parameters.SearchAction.Trim();
            query = query.Where(l => l.Action.Contains(searchAction));
        }

        if (!string.IsNullOrWhiteSpace(parameters.SearchDescription))
        {
            var searchDescription = parameters.SearchDescription.Trim().ToLower();
            query = query.Where(l => l.Description.ToLower().Contains(searchDescription));
        }

        var totalCount = await query.CountAsync();

        var logs = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return Ok(new PagedResult<ActivityLog>
        {
            Items = logs,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        });
    }
}