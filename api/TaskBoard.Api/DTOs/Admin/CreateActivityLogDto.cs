namespace TaskBoard.Api.DTOs.Admin;

public class CreateActivityLogDto
{

    
    public int UserId { get; set; }
    public string UserMail { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
    public int? TaskId { get; set; }
    public string? IpAddress { get; set; }
}


