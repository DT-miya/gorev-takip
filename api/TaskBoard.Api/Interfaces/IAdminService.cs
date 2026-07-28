using TaskBoard.Api.DTOs.Admin;

namespace TaskBoard.Api.Interfaces;

public interface IAdminService 
{
    Task<List<UserDto>> GetAllUsersAsync();

    Task<StatsDto> GetStatsAsync();
    Task<List<UserDto>> UpdateUserRoleAsync(int userId, string newRole, int currentAdminId);

    // Proje İşlemleri
    Task<List<ProjectDto>> GetAllProjectsAsync(int? limit = null);
    Task<PagedResponseDto<ProjectDto>> GetProjectsPageAsync(
        int page,
        int pageSize,
        string? search,
        int? minColumns,
        int? minTasks,
        int? minMembers,
        bool showArchived);
    Task<bool> DeleteProjectAsync(int projectId);
    Task<bool> ArchiveProjectAsync(int projectId);
    Task<bool> UnarchiveProjectAsync(int projectId);

}

