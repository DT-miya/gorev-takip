using TaskBoard.Api.DTOs.Dashboard;

namespace TaskBoard.Api.Interfaces;

public interface IDashboardService 
{
    Task <List<AssignedTaskDto>> GetAssignedTasksAsync(int userId);
}

