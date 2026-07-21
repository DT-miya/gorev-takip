namespace TaskBoard.Api.DTOs.Project;

public class MemberDto
{
    public string FullName { get; set;} = null!;
    public int UserId { get; set;}
    public string Email { get; set;} = null!;
    public string Role { get; set;} = null!;
}