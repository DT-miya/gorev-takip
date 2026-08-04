using TaskBoard.Api.DTOs.Admin;

namespace TaskBoard.Api.Interfaces;

public interface IAdminService 
{
    Task<PagedResult<UserDto>> GetAllUsersAsync(UserFilterParametersDto parameters);

    Task<StatsDto> GetStatsAsync();
    Task<UserDto> UpdateUserRoleAsync(int userId, string newRole, int currentAdminId, string currentAdminRole);

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

        Task<bool> DeleteProjectAsync(int projectId, int currentAdminId, string currentAdminName);

        Task<bool> ArchiveProjectAsync(int projectId, int currentAdminId, string currentAdminName);

        Task<bool> UnarchiveProjectAsync(int projectId, int currentAdminId, string currentAdminName);

}

