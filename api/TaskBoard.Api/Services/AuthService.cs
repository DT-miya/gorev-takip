using TaskBoard.Api.DTOs.Auth;
using TaskBoard.Api.Interfaces;

namespace TaskBoard.Api.Services;

public class AuthService : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        return new AuthResponse
        {
            Success = false,
            Message = "Not implemented"
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        return new AuthResponse
        {
            Success = false,
            Message = "Not implemented"
        };
    }
}