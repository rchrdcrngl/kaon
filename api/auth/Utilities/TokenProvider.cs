using System.Security.Claims;
using System.Text;
using AuthenticationAPI.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationAPI.Utilities;

internal sealed class TokenProvider : ITokenProvider
{
    private readonly IConfiguration _configuration;

    public TokenProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        string secretKey = _configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([ new Claim(ClaimTypes.Name, user.Username),
                                            new Claim(ClaimTypes.Role, user.Role.ToString()) ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = credentials
        };

        return new JsonWebTokenHandler().CreateToken(token);
    }

    public async Task<bool> ValidateToken(string token)
    {
        string secretKey = _configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var tokenHandler = new JsonWebTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var result = await tokenHandler.ValidateTokenAsync(token, validationParameters);
            return result.IsValid;
        }
        catch
        {
            return false;
        }
    }
}


public interface ITokenProvider
{
    string GenerateToken(User user);
    Task<bool> ValidateToken(string token);
}