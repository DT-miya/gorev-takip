using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.DTOs.Admin;
using TaskBoard.Api.Interfaces;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
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
        var currentAdminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var currentAdminName = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";

        var result = await _adminService.UpdateUserRoleAsync(userId, dto.Role, currentAdminId, currentAdminName);
        return Ok(result);
    }

    // 4. Tüm Projeler (Limitli / Basit Liste)
    [HttpGet("projects/all")]
    public async Task<IActionResult> GetAllProjects([FromQuery] int? limit = null)
    {
        var result = await _adminService.GetAllProjectsAsync(limit);
        return Ok(result);
    }

    // 🔍 5. Sayfalanmış Proje Listesi (ÇİFT FİLTRELİ)
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

    [HttpGet("projects/{projectId}/tasks")]
public async Task<IActionResult> GetProjectTasks(int projectId)
{
    var tasks = await _adminService.GetProjectTasksAsync(projectId);
    return Ok(tasks);
}

[HttpGet("projects/{projectId}/members")]
public async Task<IActionResult> GetProjectMembers(int projectId)
{
    var members = await _adminService.GetProjectMembersAsync(projectId);
    return Ok(members);
}

    // 6. Proje Silme
    [HttpDelete("projects/{projectId:int}")]
    public async Task<IActionResult> DeleteProject(int projectId)
    {
        var currentAdminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var currentAdminName = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";

        var success = await _adminService.DeleteProjectAsync(projectId, currentAdminId, currentAdminName);
        if (!success)
            return NotFound("Proje bulunamadı.");

        return Ok(new { message = "Proje başarıyla silindi." });
    }

    // 7. Proje Arşivleme (Angular PUT atıyor, HttpPut yapıldı ✅)
    [HttpPut("projects/{projectId:int}/archive")]
    public async Task<IActionResult> ArchiveProject(int projectId)
    {
        var success = await _adminService.ArchiveProjectAsync(projectId);
        if (!success)
            return NotFound("Proje bulunamadı.");

        return Ok(new { message = "Proje arşivlendi." });
    }

    // 8. Proje Arşivden Çıkarma (Angular PUT atıyor, HttpPut yapıldı ✅)
    [HttpPut("projects/{projectId:int}/unarchive")]
    public async Task<IActionResult> UnarchiveProject(int projectId)
    {
        var success = await _adminService.UnarchiveProjectAsync(projectId);
        if (!success)
            return NotFound("Proje bulunamadı.");

        return Ok(new { message = "Proje arşivden çıkarıldı." });
    }
}