using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BLL.DTO;
using BLL.Services;
using BLL.Services.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _users;
    private readonly JwtTokenService _jwt;
    private readonly IConfiguration _cfg;

    public AuthController(UserService users, JwtTokenService jwt, IConfiguration cfg)
    {
        _users = users; _jwt = jwt; _cfg = cfg;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        var user = await _users.AuthenticateAsync(req.Username, req.Password);
        if (user is null) return Unauthorized(new { message = "Sai username/password" });

        var minutes = int.TryParse(_cfg["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
        var token = _jwt.CreateAccessToken(user, TimeSpan.FromMinutes(minutes));

        return Ok(new AuthResponse
        {
            AccessToken = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(minutes),
            Username = user.UserName,
            Role = user.Role
        });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        var username = User.Identity?.Name;

        var role = User.FindFirst(ClaimTypes.Role)?.Value
               ?? User.FindFirst("role")?.Value; // phòng khi bạn dùng custom "role"

        return Ok(new { id, username, role });
    }
}
