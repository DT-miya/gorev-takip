namespace TaskBoard.Api.DTOs.Project;

public class ProjectResponse
{
    public int Id { get; set;}
    public string Name { get; set;} = null!;
    public string? Description { get; set;}
    public string OwnerName { get; set;} = null!;
    public int MemberCount { get; set;}
}