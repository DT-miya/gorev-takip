using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.Data.Entities;
using TaskBoard.Api.DTOs;
using TaskBoard.Api.DTOs.Board;
using TaskBoard.Api.Exceptions;
using TaskBoard.Api.Interfaces;

namespace TaskBoard.Api.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly IProjectService _projectService;

        public TaskService(AppDbContext context, IProjectService projectService)
        {
            _context = context;
            _projectService = projectService;
        }

        public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, int userId)
        {
            var column = await _context.BoardColumns.FindAsync(request.ColumnId)
                ?? throw new KeyNotFoundException("Kolon bulunamadı.");

            await _projectService.EnsureMemberAsync(column.ProjectId, userId);

            int maxOrder = await _context.Tasks
                .Where(t => t.ColumnId == request.ColumnId)
                .Select(t => (int?)t.Order)
                .MaxAsync() ?? 0;

            var task = new TaskItem
            {
                ColumnId = request.ColumnId,
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                AssigneeId = request.AssigneeId,
                Order = maxOrder + 1
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return await MapToResponseAsync(task.Id);
        }

        public async Task<TaskResponse> UpdateAsync(int taskId, UpdateTaskRequest request, int userId)
        {
            var task = await _context.Tasks.Include(t => t.Column).FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new KeyNotFoundException("Görev bulunamadı.");

            await _projectService.EnsureMemberAsync(task.Column.ProjectId, userId);

            task.Title = request.Title;
            task.Description = request.Description;
            task.Priority = request.Priority;
            task.AssigneeId = request.AssigneeId;
            task.DueDate = request.DueDate;

            await _context.SaveChangesAsync();
            return await MapToResponseAsync(task.Id);
        }

        public async Task<bool> DeleteAsync(int taskId, int userId)
        {
            var task = await _context.Tasks.Include(t => t.Column).FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new KeyNotFoundException("Görev bulunamadı.");

            await _projectService.EnsureMemberAsync(task.Column.ProjectId, userId);

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveAsync(int taskId, MoveTaskRequest request, int userId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null) return false;

            int sourceColumnId = task.ColumnId;
            int targetColumnId = request.TargetColumnId;
            int newOrder = request.NewOrder;

            // SENARYO 1: Farklı Kolona Aktarım Yapılıyor
            if (sourceColumnId != targetColumnId)
            {
                var sourceTasks = await _context.Tasks
                    .Where(t => t.ColumnId == sourceColumnId && t.Id != taskId)
                    .OrderBy(t => t.Order)
                    .ToListAsync();

                for (int i = 0; i < sourceTasks.Count; i++)
                {
                    sourceTasks[i].Order = i + 1;
                }

                task.ColumnId = targetColumnId;
                task.Order = newOrder;

                var targetTasks = await _context.Tasks
                    .Where(t => t.ColumnId == targetColumnId && t.Id != taskId)
                    .OrderBy(t => t.Order)
                    .ToListAsync();

                int insertIndex = Math.Clamp(newOrder - 1, 0, targetTasks.Count);
                targetTasks.Insert(insertIndex, task);

                for (int i = 0; i < targetTasks.Count; i++)
                {
                    targetTasks[i].Order = i + 1;
                }
            }
            // SENARYO 2: Aynı Kolon İçinde Yer Değiştiriliyor
            else
            {
                var columnTasks = await _context.Tasks
                    .Where(t => t.ColumnId == sourceColumnId && t.Id != taskId)
                    .OrderBy(t => t.Order)
                    .ToListAsync();

                int insertIndex = Math.Clamp(newOrder - 1, 0, columnTasks.Count);
                columnTasks.Insert(insertIndex, task);

                for (int i = 0; i < columnTasks.Count; i++)
                {
                    columnTasks[i].Order = i + 1;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // KRİTİK ENDPOINT İÇİN ÇALIŞAN METOD
        public async Task<BoardFullResponse> GetFullBoardAsync(int projectId, int userId, bool skipMemberCheck = false)
        {

             var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsArchived)
                 ?? throw new ForbiddenException("Proje bulunamadı veya arşivlenmiş.");
            
            if (!skipMemberCheck)
            {
                await _projectService.EnsureMemberAsync(projectId, userId);
            }

            

            var columns = await _context.BoardColumns
                .Where(c => c.ProjectId == projectId)
                .OrderBy(c => c.Order)
                .Include(c => c.Tasks.OrderBy(t => t.Order))
                    .ThenInclude(t => t.Assignee)
                .ToListAsync();

            return new BoardFullResponse
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                Columns = columns.Select(c => new ColumnResponse
                {
                    Id = c.Id,
                    ProjectId = c.ProjectId,
                    Title = c.Name,
                    Order = c.Order,
                    Tasks = c.Tasks.OrderBy(t => t.Order).Select(t => new TaskResponse
                    {
                        Id = t.Id,
                        ColumnId = t.ColumnId,
                        Title = t.Title,
                        Description = t.Description,
                        Priority = t.Priority,
                        Order = t.Order,
                        AssigneeId = t.AssigneeId,
                        AssigneeName = t.Assignee?.FullName,
                        DueDate = t.DueDate
                    }).ToList()
                }).ToList()
            };
        }

        private async Task<TaskResponse> MapToResponseAsync(int taskId)
        {
            var t = await _context.Tasks.Include(x => x.Assignee).FirstAsync(x => x.Id == taskId);
            return new TaskResponse
            {
                Id = t.Id,
                ColumnId = t.ColumnId,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Order = t.Order,
                AssigneeId = t.AssigneeId,
                AssigneeName = t.Assignee?.FullName,
                DueDate = t.DueDate
            };
        }
    }
}