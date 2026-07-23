public class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = "Medium";
    public int? AssigneeId { get; set; }
    public DateTime? DueDate { get; set; }
}