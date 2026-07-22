using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.Data.Entities;
using TaskBoard.Api.DTOs;
using TaskBoard.Api.Interfaces;

namespace TaskBoard.Api.Services
{
    public class ColumnService : IColumnService
    {
        private readonly AppDbContext _context;
        private readonly IProjectService _projectService;

        public ColumnService(AppDbContext context, IProjectService projectService)
        {
            _context = context;
            _projectService = projectService;
        }

        public async Task<List<ColumnResponse>> GetByProjectIdAsync(int projectId, int userId)
        {
            await _projectService.EnsureMemberAsync(projectId, userId);

            return await _context.BoardColumns
                .Where(c => c.ProjectId == projectId)
                .OrderBy(c => c.Order)
                .Select(c => new ColumnResponse { Id = c.Id, ProjectId = c.ProjectId, Title = c.Name, Order = c.Order })
                .ToListAsync();
        }

        public async Task<ColumnResponse> CreateAsync(CreateColumnRequest request, int userId)
        {
            await _projectService.EnsureMemberAsync(request.ProjectId, userId);

            int maxOrder = await _context.BoardColumns
                .Where(c => c.ProjectId == request.ProjectId)
                .Select(c => (int?)c.Order)
                .MaxAsync() ?? 0;

            //BoardColumn Entity'si kullanılıyor
            var column = new BoardColumn
            {
                ProjectId = request.ProjectId,
                Name = request.Title,
                Order = maxOrder + 1
            };

            _context.BoardColumns.Add(column);
            await _context.SaveChangesAsync();

            return new ColumnResponse { Id = column.Id, ProjectId = column.ProjectId, Title = column.Name, Order = column.Order };
        }

        public async Task<ColumnResponse> UpdateTitleAsync(int columnId, UpdateColumnRequest request, int userId)
        {
            var column = await _context.BoardColumns.FindAsync(columnId)
                ?? throw new KeyNotFoundException("Kolon bulunamad�.");

            await _projectService.EnsureMemberAsync(column.ProjectId, userId);

            column.Name = request.Title;
            await _context.SaveChangesAsync();

            return new ColumnResponse { Id = column.Id, ProjectId = column.ProjectId, Title = column.Name, Order = column.Order };
        }

        public async Task<bool> DeleteAsync(int columnId, int userId)
        {
            var column = await _context.BoardColumns.FindAsync(columnId)
                ?? throw new KeyNotFoundException("Kolon bulunamad�.");

            await _projectService.EnsureMemberAsync(column.ProjectId, userId);

            _context.BoardColumns.Remove(column);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReorderAsync(ReorderColumnsRequest request, int userId)
        {
            await _projectService.EnsureMemberAsync(request.ProjectId, userId);

            var columns = await _context.BoardColumns
                .Where(c => c.ProjectId == request.ProjectId)
                .ToListAsync();

            for (int i = 0; i < request.OrderedColumnIds.Count; i++)
            {
                var col = columns.FirstOrDefault(c => c.Id == request.OrderedColumnIds[i]);
                if (col != null)
                {
                    col.Order = i + 1;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}