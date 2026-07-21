namespace TaskBoard.Api.DTOs.Project;

public class CreateProjectRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}