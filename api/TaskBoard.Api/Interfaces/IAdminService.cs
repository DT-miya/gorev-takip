using TaskBoard.Api.DTOs.Admin;

namespace TaskBoard.Api.Interfaces;

public interface IAdminService 
{
    Task<PagedResult<UserDto>> GetAllUsersAsync(UserFilterParametersDto parameters);

    Task<StatsDto> GetStatsAsync();
    Task<UserDto> UpdateUserRoleAsync(int userId, string newRole, int currentAdminId, string currentAdminRole);

    // Proje İşlemleri
    Task<List<ProjectDto>> GetAllProjectsAsync();
    Task<bool> DeleteProjectAsync(int projectId, int currentAdminId, string currentAdminName);

}

