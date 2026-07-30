using System.Threading.Tasks;
using TaskBoard.Api.DTOs.Profile;

namespace TaskBoard.Api.Interfaces;

public interface IProfileService
{
    Task<UserProfileDto> GetProfileAsync(int userId);
    Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
}