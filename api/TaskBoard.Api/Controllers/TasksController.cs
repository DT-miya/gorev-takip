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
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private int GetCurrentUserId() => 
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskRequest request) =>
            Ok(await _taskService.CreateAsync(request, GetCurrentUserId()));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTaskRequest request) =>
            Ok(await _taskService.UpdateAsync(id, request, GetCurrentUserId()));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await _taskService.DeleteAsync(id, GetCurrentUserId()));

        [HttpPost("{id}/move")]
        public async Task<IActionResult> Move(int id, MoveTaskRequest request) =>
            Ok(await _taskService.MoveAsync(id, request, GetCurrentUserId()));
    }
}