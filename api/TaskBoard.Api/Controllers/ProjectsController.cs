using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskBoard.Api.DTOs.Project;
using System.Security.Claims;
using TaskBoard.Api.Interfaces;
using TaskBoard.Api.Extensions;
using TaskBoard.Api.DTOs.Admin;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

     private readonly IActivityLogService _logService;
   

    public ProjectsController(IProjectService projectService, IActivityLogService logService)
    {
        _projectService = projectService;
        _logService = logService;
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


    string clientIp = HttpContext.GetClientIpAddress();

    var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                ?? "Kullanıcı";

        // 🚀 GİRİŞ İŞLEMİ LOG KAYDI
        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = userId,
            UserName = userName,
            Action = "USER_UPDATE_PROJECT",
            Description = $"{userName}, #{project.Id} Id'li {project.Name} projesini güncelledi.",
            IpAddress = clientIp
        });
    

    return Ok(project);
}
[HttpPost]
public async Task<IActionResult> Create(CreateProjectRequest request)
{
    var userId = GetUserId();
    var project = await _projectService.CreateAsync(request, userId);

 string clientIp = HttpContext.GetClientIpAddress();

    var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                ?? "Kullanıcı";

        // 🚀 GİRİŞ İŞLEMİ LOG KAYDI
        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = userId,
            UserName = userName,
            Action = "USER_CREATE_PROJECT",
            Description = $"{userName}, #{project.Id} Id'li {project.Name} projesini oluşturdu.",
            IpAddress = clientIp
        });


    return Ok(project);
}
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var userId = GetUserId();
    await _projectService.DeleteAsync(id, userId);

string clientIp = HttpContext.GetClientIpAddress();

    var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                ?? "Kullanıcı";

        // 🚀 GİRİŞ İŞLEMİ LOG KAYDI
        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = userId,
            UserName = userName,
            Action = "USER_DELETE_PROJECT",
            Description = $"{userName}, #{id} Id'li projeyi sildi.",
            IpAddress = clientIp
        });

    
    return NoContent();
}

[HttpGet("{id}/members")]
public async Task<IActionResult> GetMembers(int id)
    {
        var userId = GetUserId();
        var members = await _projectService.GetMembersAsync(id, userId, skipmemberCheck: false);
        return Ok(members);
    }

[HttpPost("{id}/members")]

public async Task<IActionResult> AddMember(int id, AddMemberRequest request)
    {
        var userId = GetUserId();
        var members = await _projectService.AddMemberAsync(id, request, userId);

          string clientIp = HttpContext.GetClientIpAddress();

    var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                ?? "Kullanıcı";

        // 🚀 GİRİŞ İŞLEMİ LOG KAYDI
        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = userId,
            UserName = userName,
            Action = "USER_ADD_PROJECT_MEMBER",
            Description = $"{userName}, #{id} Id'li projeye #{request.Email} kullanıcısını ekledi.",
            IpAddress = clientIp
        });



        return Ok(members);
    }

[HttpDelete("{id}/members/{memberUserId}")]
public async Task<IActionResult> RemoveMember(int id, int memberUserId)
{
    var userId = GetUserId();
    var members = await _projectService.RemoveMemberAsync(id, memberUserId, userId);


    string clientIp = HttpContext.GetClientIpAddress();
    var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                ?? "Kullanıcı";

        // 🚀 GİRİŞ İŞLEMİ LOG KAYDI
        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = userId,
            UserName = userName,
            Action = "USER_REMOVE_PROJECT_MEMBER",
            Description = $"{userName}, #{id} Id'li projeden #{memberUserId} Id'li kullanıcıyı çıkardı.",
            IpAddress = clientIp
        });



    return Ok(members);
}
}