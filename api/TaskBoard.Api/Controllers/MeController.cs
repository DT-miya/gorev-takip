using TaskBoard.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public class MeController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public MeController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpGet("assigned-tasks")]
    public async Task<IActionResult> GetAssignedTasks()
    {
        var userId = GetUserId();
        var tasks = await _dashboardService.GetAssignedTasksAsync(userId);
        return Ok(tasks);
    }
}
