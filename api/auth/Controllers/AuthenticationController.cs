using AuthenticationAPI.Models;
using AuthenticationAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthenticationController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var result = await _authService.RegisterAsync(model.Username, model.Password, model.Email, model.Role);
        if (result)
        {
            return Ok(new { message = "User registered successfully" });
        }
        return BadRequest(new { message = "Username already exists" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var (accessToken, refreshToken) = await _authService.LoginAsync(model.Username, model.Password);
        if (accessToken != null && refreshToken != null)
        {
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }
        return Unauthorized(new { message = "Invalid username or password" });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenModel model)
    {
        var (accessToken, refreshToken) = await _authService.RefreshTokenAsync(model.RefreshToken);
        if (accessToken != null && refreshToken != null)
        {
            return Ok(new { accessToken, refreshToken });
        }
        return BadRequest(new { message = "Invalid refresh token" });
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] ValidateModel model)
    {
        var isValid = await _authService.ValidateTokenAsync(model.AccessToken);
        if (isValid)
        {
            return Ok(new { message = "Token is valid" });
        }
        return BadRequest(new { message = "Invalid token" });
    }
}

public class RegisterModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required UserRole Role { get; set; }
}

public class LoginModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class ValidateModel
{
    public required string AccessToken { get; set; }
}

public class RefreshTokenModel
{
    public required string RefreshToken { get; set; }
}