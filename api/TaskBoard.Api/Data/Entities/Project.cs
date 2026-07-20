namespace TaskBoard.Api.Data.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int OwnerId { get; set;}
    public DateTime CreatedAt { get; set;} = DateTime.UtcNow;

    public User Owner {get; set; } = null!;
    public List<ProjectMember> Members { get; set;} = new();
    public List<BoardColumn> Columns {get; set;} = new();
}