namespace TaskBoard.Api.Data.Entities;

public static class ProjectRoles
{
    public const string Owner = "Owner";
    public const string Member = "Member";
}

public class ProjectMember
{
    public int Id { get; set;}
    public int ProjectId { get; set;}
    public int UserId { get; set;}
    public string Role {get; set;} = ProjectRoles.Member;
    public DateTime JoinedAt {get; set;} = DateTime.UtcNow;


    public Project Project {get; set;} = null!;
    public User User {get; set;} = null!;
}