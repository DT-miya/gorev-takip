using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.Interfaces;
using TaskBoard.Api.DTOs.Admin;
using TaskBoard.Api.Data.Entities;
using System.Security.Claims;
using TaskBoard.Api.Data;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Extensions;

namespace TaskBoard.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
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

            // DTO Nesnesi ile Log Kaydı
            await _logService.LogAsync(new CreateActivityLogDto
            {
                UserId = currentAdminId,
                UserMail = currentAdminName,
                Action = "ROLE_CHANGE",
                Description = $"{currentAdminName}, #{id} Id'li kullanıcının rolünü '{request.Role}' olarak değiştirdi.",
                IpAddress = clientIp
            });

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
            var currentAdminId = GetUserId();
            var currentAdminName = User.FindFirst(ClaimTypes.Name)?.Value
                        ?? User.FindFirst(ClaimTypes.Email)?.Value 
                        ?? "Admin";
            var result = await _adminService.DeleteProjectAsync(id, currentAdminId, currentAdminName);
            if (!result)
                return NotFound(new { message = "Proje bulunamadı." });

            string clientIp = HttpContext.GetClientIpAddress();

            // DTO Nesnesi ile Log Kaydı
            await _logService.LogAsync(new CreateActivityLogDto
            {
                UserId = currentAdminId,
                UserMail = currentAdminName,
                Action = "PROJECT_DELETE",
                Description = $"{currentAdminName}, #{id} Id'li projeyi sildi.",
                IpAddress = clientIp
            });

            return Ok(new { message = "Proje başarıyla silindi." });
        }

        [HttpGet("projects/{projectId}/tasks")]
        public async Task<IActionResult> GetProjectTasks(int projectId)
        {
            var userId = GetUserId();
            var board = await _taskService.GetFullBoardAsync(projectId, userId, skipmemberCheck: true);
            return Ok(board);  
        }

        [HttpGet("projects/{projectId}/members")]
        public async Task<IActionResult> GetProjectMembers(int projectId)
        {
            var userId = GetUserId();
            var members = await _projectService.GetMembersAsync(projectId, userId,skipmemberCheck: true);
            return Ok(members);
        }

        [HttpPut("projects/{id}/archive")]
        public async Task<IActionResult> ArchiveProject(int id)
        {

             var currentAdminId = GetUserId();
            var currentAdminName = User.FindFirst(ClaimTypes.Name)?.Value
                        ?? User.FindFirst(ClaimTypes.Email)?.Value 
                        ?? "Admin";
            var result = await _adminService.ArchiveProjectAsync(id, currentAdminId, currentAdminName);
            if (!result)
                return NotFound(new { message = "Proje bulunamadı." });


                 string clientIp = HttpContext.GetClientIpAddress();

            // DTO Nesnesi ile Log Kaydı
            await _logService.LogAsync(new CreateActivityLogDto
            {
                UserId = currentAdminId,
                UserMail = currentAdminName,
                Action = "PROJECT_ARCHIVE",
                Description = $"{currentAdminName}, #{id} Id'li projeyi arşivledi.",
                IpAddress = clientIp
            });

            return Ok(new { message = "Proje başarıyla arşivlendi." });
        }

        [HttpPut("projects/{id}/unarchive")]
        public async Task<IActionResult> UnarchiveProject(int id)
        {

             var currentAdminId = GetUserId();
            var currentAdminName = User.FindFirst(ClaimTypes.Name)?.Value
                        ?? User.FindFirst(ClaimTypes.Email)?.Value 
                        ?? "Admin";
            var result = await _adminService.UnarchiveProjectAsync(id, currentAdminId, currentAdminName);
            if (!result)
                return NotFound(new { message = "Proje bulunamadı." });

            return Ok(new { message = "Proje geri getirildi." });
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs([FromQuery] LogFilterParametersDto parameters)
        {
            var query = _context.ActivityLogs.AsNoTracking().AsQueryable();

            // ARAMA FİLTRESİ
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
}