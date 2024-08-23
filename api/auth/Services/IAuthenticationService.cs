using AuthenticationAPI.Models;

namespace AuthenticationAPI.Services;

public interface IAuthenticationService
{
    Task<bool> RegisterAsync(string username, string password, string email, UserRole role);
    Task<(string? AccessToken, string? RefreshToken)> LoginAsync(string username, string password);
    Task LogoutAsync(string token);
    Task<bool> ValidateTokenAsync(string token);
    Task<(string? AccessToken, string? RefreshToken)> RefreshTokenAsync(string refreshToken);
}