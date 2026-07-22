using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.DTOs.Auth;
using TaskBoard.Api.Interfaces;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

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

        return Ok(result);
    }


    [HttpPost("logout")]
    public IActionResult Logout()
    {
      
        
        return Ok(new { success = true, message = "��k�� yap�ld�." });
    }

}