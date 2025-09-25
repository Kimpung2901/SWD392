using System.ComponentModel.DataAnnotations;
using BLL.DTO.UserDto;
using BLL.Services;
using BLL.Services.Jwt;
using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _users;
    private readonly JwtTokenService _jwt;
    private readonly DollDbContext _db;
    private readonly IConfiguration _cfg;

    public AuthController(UserService users, JwtTokenService jwt, DollDbContext db, IConfiguration cfg)
    {
        _users = users ?? throw new ArgumentNullException(nameof(users));
        _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _cfg = cfg ?? throw new ArgumentNullException(nameof(cfg));
    }

    public class LoginRequest
    {
        [Required] public string Username { get; set; } = null!;
        [Required] public string Password { get; set; } = null!;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        // Validate input tránh req null
        if (req == null) return BadRequest(new { message = "Body null" });
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Xác thực user
        var user = await _users.AuthenticateAsync(req.Username, req.Password);
        if (user is null) return Unauthorized(new { message = "username/password wrong!" });

        // Sinh access token
        var minutes = int.TryParse(_cfg["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
        var accessToken = _jwt.CreateAccessToken(user, TimeSpan.FromMinutes(minutes));

        // Tạo refresh token & lưu DB
        var refresh = new RefreshToken
        {
            UserID = user.UserID,
            Token = Guid.NewGuid().ToString("N"),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
        };
        _db.RefreshTokens.Add(refresh);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            accessToken,
            expiresAt = DateTime.UtcNow.AddMinutes(minutes),
            refreshToken = refresh.Token,
            username = user.UserName,
            role = user.Role
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return BadRequest(new { message = "refreshToken missing" });

        var token = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null || token.Revoked != null || token.Expires <= DateTime.UtcNow)
            return Unauthorized(new { message = "Refresh token is invalid or expired" });

        var minutes = int.TryParse(_cfg["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
        var newAccess = _jwt.CreateAccessToken(token.User, TimeSpan.FromMinutes(minutes));

        return Ok(new { accessToken = newAccess, expiresAt = DateTime.UtcNow.AddMinutes(minutes) });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return BadRequest(new { message = "refreshToken missing!!" });

        var token = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        if (token == null) return NotFound(new { message = "Refresh token is not exist!" });

        token.Revoked = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { message = "revoked refresh token" });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
              ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        var username = User.Identity?.Name;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
               ?? User.FindFirst("Role")?.Value;

        return Ok(new { id, username, role });
    }
    [HttpPost("change_password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.CurrentPassword) ||
            string.IsNullOrWhiteSpace(req.NewPassword) ||
            req.NewPassword != req.ConfirmPassword)
            return BadRequest(new { message = "Invalid data" });

        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        var ok = await _users.ChangePasswordAsync(userId, req.CurrentPassword, req.NewPassword);
        return ok ? Ok(new { message = "change password successfully!" })
                  : BadRequest(new { message = "Current password is incorrect" });
    }

}
