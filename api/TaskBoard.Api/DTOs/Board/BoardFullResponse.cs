namespace TaskBoard.Api.DTOs.Board;

public class BoardFullResponse
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    
    
    public List<ColumnResponse> Columns { get; set; } = new();
}