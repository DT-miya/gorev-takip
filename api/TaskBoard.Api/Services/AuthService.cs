using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskBoard.Api.Data;       // AppDbContext ve Entities burada yer alýyor
using TaskBoard.Api.DTOs.Auth;
using TaskBoard.Api.Interfaces;
using TaskBoard.Api.Data.Entities;
using BCrypt.Net;

namespace TaskBoard.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            return new AuthResponse { Success = false, Message = "Bu e-posta adresi zaten kullanýmda." };
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponse { Success = true, Message = "Kayýt baþarýlý." };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return new AuthResponse { Success = false, Message = "E-posta veya þifre hatalý." };
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return new AuthResponse { Success = false, Message = "E-posta veya þifre hatalý." };
        }

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Giriþ baþarýlý.",
            Token = token
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "BuCokGizliVeUzunBirSecretKey123456!");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}