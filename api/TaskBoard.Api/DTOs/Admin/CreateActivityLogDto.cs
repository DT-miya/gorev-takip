namespace TaskBoard.Api.DTOs.Admin;

public class CreateActivityLogDto
{

    
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
    public int? TaskId { get; set; }
    public string? IpAddress { get; set; }
}


public class LogFilterParametersDto
{
    public string? Search { get; set; } // Kullanıcı adı, action veya açıklama araması
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}