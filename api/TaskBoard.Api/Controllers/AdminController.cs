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
}