using TaskBoard.Api.DTOs.Admin;

namespace TaskBoard.Api.Interfaces;

public interface IAdminService 
{
    Task<List<UserDto>> GetAllUsersAsync();

    Task<StatsDto> GetStatsAsync();
    Task<List<UserDto>> UpdateUserRoleAsync(int userId, string newRole, int currentAdminId);

    // Proje İşlemleri
    Task<List<ProjectDto>> GetAllProjectsAsync();
    Task<bool> DeleteProjectAsync(int projectId);

}

