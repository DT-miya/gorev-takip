public class TaskResponse
{
    public int Id { get; set; }
    public int ColumnId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = string.Empty;
    public int Order { get; set; }
    public int? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }

    public DateTime? DueDate { get; set; }
}