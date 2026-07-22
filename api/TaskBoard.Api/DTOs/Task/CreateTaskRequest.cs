public class CreateTaskRequest
{
    public int ColumnId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = "Medium"; // Low, Medium, High
    public int? AssigneeId { get; set; }
}