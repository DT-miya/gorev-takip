public class ColumnResponse
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }

    public List<TaskResponse> Tasks { get; set; } = new();
}