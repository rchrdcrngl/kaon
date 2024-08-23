using AuthenticationAPI.Models;
using AuthenticationAPI.Utilities;
using AuthenticationAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AuthenticationAPI.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ITokenProvider _tokenProvider;

    public AuthenticationService(AuthenticationContext context, IPasswordHasher passwordHasher, IConfiguration configuration, ITokenProvider tokenProvider)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _tokenProvider = tokenProvider;
    }

    public async Task<bool> RegisterAsync(string username, string password, string email, UserRole role)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            return false;
        }

        var user = new User
        {
            Username = username,
            PasswordHash = _passwordHasher.Hash(password),
            Email = email,
            Role = role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(string? AccessToken, string? RefreshToken)> LoginAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
        {
            return (null, null);
        }

        var accessToken = _tokenProvider.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        if (await UpdateUserRefreshToken(user, refreshToken))
        {
            return (accessToken, refreshToken);
        }

        return (null, null);
    }

    public Task LogoutAsync(string token)
    {
        // In a stateless JWT-based authentication, logout is typically handled client-side
        // by removing the token from storage. No server-side action is required.
        return Task.CompletedTask;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            return await Task.Run(() => _tokenProvider.ValidateToken(token));
        }
        catch (Exception)
        {
            // Log the exception if needed
            return false;
        }
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<bool> UpdateUserRefreshToken(User user, string refreshToken)
    {
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Set expiry to 7 days
        _context.Update(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<(string? AccessToken, string? RefreshToken)> RefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return (null, null);
        }

        var newRefreshToken = GenerateRefreshToken();
        if (await UpdateUserRefreshToken(user, newRefreshToken))
        {
            var newAccessToken = _tokenProvider.GenerateToken(user);
            return (newAccessToken, newRefreshToken);
        }

        return (null, null);
    }
}