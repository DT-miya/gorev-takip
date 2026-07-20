namespace TaskBoard.Api.Data.Entities;

public static class TaskPriorities
{
    public const string Low = "Low";
    public const string Medium = "Medium";
    public const string High = "High";
}

public class TaskItem
{
    public int Id { get; set; }
    public int ColumnId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int? AssigneeId { get; set; }
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = TaskPriorities.Medium;
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

   
    public BoardColumn Column { get; set; } = null!;
    public User? Assignee { get; set; }
}