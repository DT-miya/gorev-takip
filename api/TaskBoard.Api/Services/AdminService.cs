using TaskBoard.Api.Data;
using TaskBoard.Api.DTOs.Admin;
using TaskBoard.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Exceptions;

namespace TaskBoard.Api.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();
    }

    public async Task<StatsDto> GetStatsAsync()
    {
        return new StatsDto
        {
            UserCount = await _context.Users.CountAsync(),
            ProjectCount = await _context.Projects.CountAsync(),
            TaskCount = await _context.Tasks.CountAsync()
        };
    }

    public async Task<List<UserDto>> UpdateUserRoleAsync(int userId, string newRole, int currentAdminId)
{
    if (newRole != "Admin" && newRole != "User")
        throw new ConflictException("Geçersiz rol");

    if (userId == currentAdminId)
        throw new ConflictException("Kendi rolünüzü değiştiremezsiniz");

    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    if (user == null)
        throw new NotFoundException("Kullanıcı bulunamadı");

    user.Role = newRole;
    await _context.SaveChangesAsync();

    return await GetAllUsersAsync();
}
}