using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.DTOs.Dashboard;
using TaskBoard.Api.Interfaces;

namespace TaskBoard.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AssignedTaskDto>> GetAssignedTasksAsync(int userId)
    {
        var tasks = await _context.Tasks
        .Where(t => t.AssigneeId == userId && !t.Column.Project.IsArchived)
        .OrderBy(t => t.DueDate)
        .Select(t => new AssignedTaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Priority = t.Priority,
            DueDate = t.DueDate,
            ColumnName = t.Column.Name,
            ProjectId = t.Column.ProjectId,
            ProjectName = t.Column.Project.Name
        })
        .ToListAsync();
        return tasks;
    }
}