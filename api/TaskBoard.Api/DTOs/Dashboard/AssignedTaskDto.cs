namespace TaskBoard.Api.DTOs.Dashboard;

public class AssignedTaskDto
{
    public int Id { get; set;}
    public string Title { get; set;} = string.Empty;
    public string Priority { get; set;} = string.Empty;
    public DateTime? DueDate { get; set; }
    public string ColumnName { get; set;} = string.Empty;
    public int ProjectId { get; set;}
    public string ProjectName { get; set;} = string.Empty;

}