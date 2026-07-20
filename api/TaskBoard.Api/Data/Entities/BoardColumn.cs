namespace TaskBoard.Api.Data.Entities;

public class BoardColumn
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public int Order { get; set; }

    
    public Project Project { get; set; } = null!;
    public List<TaskItem> Tasks { get; set; } = new();
}