using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskBoard.Api.Data;       // AppDbContext ve Entities burada yer al�yor
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
            return new AuthResponse { Success = false, Message = "Bu e-posta adresi zaten kullan�mda." };
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

        return new AuthResponse { Success = true, Message = "Kayıt başarılı." };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return new AuthResponse { Success = false, Message = "E-posta veya şifre hatalı." };
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return new AuthResponse { Success = false, Message = "E-posta veya şifre hatalı." };
        }

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Giriş Başarılı",
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
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
