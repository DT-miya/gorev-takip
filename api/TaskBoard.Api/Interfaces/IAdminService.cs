using TaskBoard.Api.DTOs.Admin;

namespace TaskBoard.Api.Interfaces;

public interface IAdminService 
{
    Task<List<UserDto>> GetAllUsersAsync();


    // Proje İşlemleri
    Task<List<ProjectDto>> GetAllProjectsAsync();
    Task<bool> DeleteProjectAsync(int projectId);
}

