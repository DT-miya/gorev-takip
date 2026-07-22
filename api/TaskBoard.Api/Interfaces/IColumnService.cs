public interface IColumnService
{
    Task<List<ColumnResponse>> GetByProjectIdAsync(int projectId, int userId);
    Task<ColumnResponse> CreateAsync(CreateColumnRequest request, int userId);
    Task<ColumnResponse> UpdateTitleAsync(int columnId, UpdateColumnRequest request, int userId);
    Task<bool> DeleteAsync(int columnId, int userId);
    Task<bool> ReorderAsync(ReorderColumnsRequest request, int userId);
}