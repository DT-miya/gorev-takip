using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.Interfaces;
using TaskBoard.Api.DTOs.Admin;
using System.Security.Claims;

[ApiController]
[Route("api/admin")]
[Authorize(Roles="Admin")]

public class AdminController : ControllerBase

{
    private readonly IAdminService _adminService;
    private readonly ITaskService _taskService;
    private readonly IProjectService _projectService;

    public AdminController(IAdminService adminService, ITaskService taskService, IProjectService projectService)
    {
        _adminService = adminService;
        _taskService = taskService;
        _projectService = projectService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
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
        var users = await _adminService.UpdateUserRoleAsync(id, request.Role, currentAdminId);
        return Ok(users);
}

    // GET: api/admin/projects
    [HttpGet("projects")]
    public async Task<IActionResult> GetProjects([FromQuery] int? limit)
    {
        var projects = await _adminService.GetAllProjectsAsync(limit);
        return Ok(projects);
    }

    // GET: api/admin/projects/paged
    [HttpGet("projects/paged")]
    public async Task<IActionResult> GetProjectsPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? minColumns = null,
        [FromQuery] int? minTasks = null,
        [FromQuery] int? minMembers = null,
        [FromQuery] bool showArchived = false)
    {
        var projects = await _adminService.GetProjectsPageAsync(
            page,
            pageSize,
            search,
            minColumns,
            minTasks,
            minMembers,
            showArchived);

        return Ok(projects);
    }

    // DELETE: api/admin/projects/{id}
    [HttpDelete("projects/{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var result = await _adminService.DeleteProjectAsync(id);
        if (!result)
            return NotFound(new { message = "Proje bulunamadı." });

        return Ok(new { message = "Proje başarıyla silindi." });
    }
//---------------------------------------------------------------------------
    [HttpGet("projects/{projectId}/tasks")]
    public async Task<IActionResult> GetProjectTasks(int projectId)
    {
        var userId = GetUserId();
        var board = await _taskService.GetFullBoardAsync(projectId, userId);
        return Ok(board);  
    }

    [HttpGet("projects/{projectId}/members")]
    public async Task<IActionResult> GetProjectMembers(int projectId)
    {
        var userId = GetUserId();
        var members = await _projectService.GetMembersAsync(projectId, userId);
        return Ok(members);
    }

    [HttpPut("projects/{id}/archive")]
    public async Task<IActionResult> ArchiveProject(int id)
    {
        var result = await _adminService.ArchiveProjectAsync(id);
        if (!result)
            return NotFound(new { message = "Proje bulunamadı." });

        return Ok(new { message = "Proje başarıyla arşivlendi." });
    }

    [HttpPut("projects/{id}/unarchive")]
    public async Task<IActionResult> UnarchiveProject(int id)
{
    var result = await _adminService.UnarchiveProjectAsync(id);
    if (!result)
        return NotFound(new { message = "Proje bulunamadı." });

    return Ok(new { message = "Proje geri getirildi." });
}

}

