using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.Interfaces;

[ApiController]
[Route("api/admin")]
[Authorize(Roles="Admin")]

public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
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
        var result = await _adminService.DeleteProjectAsync(id);
        if (!result)
            return NotFound(new { message = "Proje bulunamadı." });

        return Ok(new { message = "Proje başarıyla silindi." });
    }
}