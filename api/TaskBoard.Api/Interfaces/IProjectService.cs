using TaskBoard.Api.DTOs.Project;

namespace TaskBoard.Api.Interfaces;

public interface IProjectService
{
    Task<List<ProjectResponse>> GetProjectResponsesAsync(int userId);

    Task<ProjectResponse> GetByIdAsync(int projectId, int userId);

    Task<ProjectResponse> CreateAsync(CreateProjectRequest request, int userId);

    Task<ProjectResponse> UpdateAsync(UpdateProjectRequest request, int userId, int projectId);

    Task DeleteAsync(int projectId, int userId);

    Task EnsureMemberAsync(int projectId, int userId);

    Task<List<MemberDto>> GetMembersAsync(int projectId, int userId);

    Task<List<MemberDto>> AddMemberAsync(int projectId, AddMemberRequest request, int userId);

    Task<List<MemberDto>> RemoveMemberAsync(int projectId, int memberUserId, int userId);


}
