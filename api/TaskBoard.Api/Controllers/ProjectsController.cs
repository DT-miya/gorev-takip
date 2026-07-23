using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskBoard.Api.DTOs.Project;
using System.Security.Claims;
using TaskBoard.Api.Interfaces;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }


private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

[HttpGet]
public async Task<IActionResult> GetMyProjects()
    {
        var userId = GetUserId();
        var projects = await _projectService.GetProjectResponsesAsync(userId);
        return Ok(projects);
    }

[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();
        var project = await _projectService.GetByIdAsync(id, userId);
        return Ok(project);
    }

[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, UpdateProjectRequest request)
{
    var userId = GetUserId();
    var project = await _projectService.UpdateAsync(request, userId, id);
    return Ok(project);
}
[HttpPost]
public async Task<IActionResult> Create(CreateProjectRequest request)
{
    var userId = GetUserId();
    var project = await _projectService.CreateAsync(request, userId);
    return Ok(project);
}
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var userId = GetUserId();
    await _projectService.DeleteAsync(id, userId);
    return NoContent();
}

[HttpGet("{id}/members")]
public async Task<IActionResult> GetMembers(int id)
    {
        var userId = GetUserId();
        var members = await _projectService.GetMembersAsync(id, userId);
        return Ok(members);
    }

[HttpPost("{id}/members")]

public async Task<IActionResult> AddMember(int id, AddMemberRequest request)
    {
        var userId = GetUserId();
        var members = await _projectService.AddMemberAsync(id, request, userId);
        return Ok(members);
    }

[HttpDelete("{id}/members/{memberUserId}")]
public async Task<IActionResult> RemoveMember(int id, int memberUserId)
{
    var userId = GetUserId();
    var members = await _projectService.RemoveMemberAsync(id, memberUserId, userId);
    return Ok(members);
}
}