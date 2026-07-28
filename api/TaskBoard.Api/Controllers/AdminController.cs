using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.Interfaces;
using TaskBoard.Api.DTOs.Admin;
using TaskBoard.Api.Data.Entities;
using System.Security.Claims;
using TaskBoard.Api.Data;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Extensions;

[ApiController]
[Route("api/admin")]
[Authorize(Roles="Admin")]

public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly AppDbContext _context;

    private readonly IActivityLogService _logService;

    public AdminController(IAdminService adminService, AppDbContext context, IActivityLogService logService)
    {
        _adminService = adminService;
        _context = context;
        _logService = logService;
    }

    // GET: api/admin/users?search=test&page=1&pageSize=20
[HttpGet("users")]
public async Task<IActionResult> GetUsers([FromQuery] UserFilterParametersDto parameters)
{
    var result = await _adminService.GetAllUsersAsync(parameters);
    return Ok(result);
}


    private int GetUserId()
    {
    return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _adminService.GetStatsAsync();
        return Ok(stats);
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, UpdateRoleRequest request)
    {
        var currentAdminId = GetUserId();
       var currentAdminName = User.FindFirst(ClaimTypes.Name)?.Value
                    ?? User.FindFirst(ClaimTypes.Email)?.Value 
                    ?? "Admin";
        var users = await _adminService.UpdateUserRoleAsync(id, request.Role, currentAdminId, currentAdminName);

         string clientIp = HttpContext.GetClientIpAddress();

         // 🚀 DTO Nesnesi ile Log Kaydı
        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = currentAdminId,
            UserName = currentAdminName,
            Action = "ROLE_CHANGE",
            Description = $"{currentAdminName}, #{id} Id'li kullanıcının rolünü '{request.Role}' olarak değiştirdi.",
            IpAddress = clientIp
        });


        return Ok(users);
}

    // GET: api/admin/projects
    [HttpGet("projects")]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _adminService.GetAllProjectsAsync();
        return Ok(projects);
    }

    // DELETE: api/admin/projects/{id}
    [HttpDelete("projects/{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var currentAdminId = GetUserId();
        var currentAdminName = User.FindFirst(ClaimTypes.Name)?.Value
                    ?? User.FindFirst(ClaimTypes.Email)?.Value 
                    ?? "Admin";
        var result = await _adminService.DeleteProjectAsync(id, currentAdminId, currentAdminName);
        if (!result)
            return NotFound(new { message = "Proje bulunamadı." });


            string clientIp = HttpContext.GetClientIpAddress();

// 🚀 DTO Nesnesi ile Log Kaydı
        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = currentAdminId,
            UserName = currentAdminName,
            Action = "PROJECT_DELETE",
            Description = $"{currentAdminName}, #{id} Id'li projeyi sildi.",
            IpAddress = clientIp
        });


        return Ok(new { message = "Proje başarıyla silindi." });
    }

[HttpGet("logs")]
public async Task<IActionResult> GetLogs([FromQuery] LogFilterParametersDto parameters)
{
    var query = _context.ActivityLogs.AsNoTracking().AsQueryable();

    // 🔍 ARAMA FİLTRESİ
    if (!string.IsNullOrWhiteSpace(parameters.Search))
    {
        var searchTerm = parameters.Search.Trim().ToLower();
        query = query.Where(l => l.UserName.ToLower().Contains(searchTerm) ||
                                 l.Action.ToLower().Contains(searchTerm) ||
                                 l.Description.ToLower().Contains(searchTerm));
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