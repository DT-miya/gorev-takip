namespace TaskBoard.Api.DTOs.Admin;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Proje Sahibi Bilgileri
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;

    // Özet İstatistikler
    public int ColumnCount { get; set; }
    public int TaskCount { get; set; }
    public int MemberCount { get; set; }
}