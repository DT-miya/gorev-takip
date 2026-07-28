using TaskBoard.Api.Data;
using TaskBoard.Api.DTOs.Admin;
using TaskBoard.Api.Interfaces;
using TaskBoard.Api.Data.Entities;

namespace TaskBoard.Api.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly AppDbContext _context;

    public ActivityLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(CreateActivityLogDto dto)
    {
        var log = new ActivityLog
        {
            UserId = dto.UserId,
            UserName = dto.UserName,
            Action = dto.Action,
            Description = dto.Description,
            ProjectId = dto.ProjectId,
            IpAddress = dto.IpAddress,
            TaskId = dto.TaskId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}