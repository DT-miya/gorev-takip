using TaskBoard.Api.DTOs;
using TaskBoard.Api.DTOs.Board;

namespace TaskBoard.Api.Interfaces
{
    public interface ITaskService
    {
        Task<TaskResponse> CreateAsync(CreateTaskRequest request, int userId);
        Task<TaskResponse> UpdateAsync(int taskId, UpdateTaskRequest request, int userId);
        Task<bool> DeleteAsync(int taskId, int userId);
        Task<bool> MoveAsync(int taskId, MoveTaskRequest request, int userId);
        Task<BoardFullResponse> GetFullBoardAsync(int projectId, int userId, bool skipmemberCheck = false);
    }
}