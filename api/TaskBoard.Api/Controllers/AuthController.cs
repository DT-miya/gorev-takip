using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.DTOs.Admin;
using TaskBoard.Api.DTOs.Auth;
using TaskBoard.Api.Extensions;
using TaskBoard.Api.Interfaces;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IActivityLogService _logService;
    public AuthController(IAuthService authService, IActivityLogService logService)
    {
        _authService = authService;
        _logService = logService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        string clientIp = HttpContext.GetClientIpAddress();

        // 🚀 KAYIT İŞLEMİ LOG KAYDI
        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = 0, // Kayıt anında ID bilinmiyorsa 0 veya AuthResponse'a Id eklenebilir
            UserMail = request.Email,
            Action = "USER_REGISTER",
            Description = $"{request.Email} e-postası ile yeni hesap oluşturuldu.",
            IpAddress = clientIp
        });

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Success || string.IsNullOrEmpty(result.Token))
        {
            return BadRequest(result);
        }

       string clientIp = HttpContext.GetClientIpAddress();

        // 🚀 GİRİŞ İŞLEMİ LOG KAYDI
        await _logService.LogAsync(new CreateActivityLogDto
        {
            UserId = 0,
            UserMail = request.Email,
            Action = "USER_LOGIN",
            Description = $"{request.Email} sisteme giriş yaptı.",
            IpAddress = clientIp
        });

        
        return Ok(result);
    }


    [HttpPost("logout")]
    public IActionResult Logout()
    {
      
        
        return Ok(new { success = true, message = "çıkış yapıldı." });
    }

}