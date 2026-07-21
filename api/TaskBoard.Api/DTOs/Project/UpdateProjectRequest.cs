namespace TaskBoard.Api.DTOs.Project;

public class UpdateProjectRequest
{
    public string Name { get; set;} = null!;
    public string? Description { get; set; }
}