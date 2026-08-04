using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.DTOs.Admin;
using TaskBoard.Api.DTOs.Profile;
using TaskBoard.Api.Extensions;
using TaskBoard.Api.Interfaces;

namespace TaskBoard.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly IActivityLogService _logService;

    public ProfileController(IProfileService profileService, IActivityLogService logService)
    {
        _profileService = profileService;
        _logService = logService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        int userId = GetCurrentUserId();
        var profile = await _profileService.GetProfileAsync(userId);
        return Ok(profile);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        int userId = GetCurrentUserId();
        var updatedProfile = await _profileService.UpdateProfileAsync(userId, dto);

        string clientIp = HttpContext.GetClientIpAddress();

        // 🚀 Loglama Controller Katmanında yapılıyor
        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = userId,
            UserMail = updatedProfile.Email,
            Action = "PROFILE_UPDATE",
            Description = $"{updatedProfile.FullName} profil bilgilerini güncelledi.",
            IpAddress = clientIp
        });

        return Ok(updatedProfile);
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        int userId = GetCurrentUserId();
        var userMail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Kullanıcı";

        await _profileService.ChangePasswordAsync(userId, dto);

        string clientIp = HttpContext.GetClientIpAddress();

        // 🚀 Loglama Controller Katmanında yapılıyor
        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = userId,
            UserMail = userMail,
            Action = "PASSWORD_CHANGE",
            Description = $"{userMail} hesabının şifresini değiştirdi.",
            IpAddress = clientIp
        });

        return Ok(new { success = true, message = "Şifreniz başarıyla değiştirildi." });
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return int.Parse(claim?.Value ?? "0");
    }
}