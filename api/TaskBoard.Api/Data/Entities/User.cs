namespace TaskBoard.Api.Data.Entities;

public class User {
    public int Id {get; set;}
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ProjectMember> Memberships { get; set; } = new();

    public string Role { get; set;} = "User";
}