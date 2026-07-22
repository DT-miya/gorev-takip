using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskBoard.Api.DTOs;
using TaskBoard.Api.Interfaces;

namespace TaskBoard.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ColumnsController : ControllerBase
    {
        private readonly IColumnService _columnService;
        private readonly ITaskService _taskService;

       public ColumnsController(IColumnService columnService, ITaskService taskService)
        {
            _columnService = columnService;
            _taskService = taskService;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            [HttpGet("project/{projectId}/full")]
        public async Task<IActionResult> GetFullBoard(int projectId) =>
            Ok(await _taskService.GetFullBoardAsync(projectId, GetCurrentUserId()));
            
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId) =>
            Ok(await _columnService.GetByProjectIdAsync(projectId, GetCurrentUserId()));

        [HttpPost]
        public async Task<IActionResult> Create(CreateColumnRequest request) =>
            Ok(await _columnService.CreateAsync(request, GetCurrentUserId()));

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTitle(int id, UpdateColumnRequest request) =>
            Ok(await _columnService.UpdateTitleAsync(id, request, GetCurrentUserId()));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await _columnService.DeleteAsync(id, GetCurrentUserId()));

        [HttpPost("reorder")]
        public async Task<IActionResult> Reorder(ReorderColumnsRequest request) =>
            Ok(await _columnService.ReorderAsync(request, GetCurrentUserId()));
    }
}