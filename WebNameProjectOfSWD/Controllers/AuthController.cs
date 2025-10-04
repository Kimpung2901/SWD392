// WebNameProjectOfSWD/Controllers/AuthController.cs
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using BLL.DTO;
using BLL.IService;
using BLL.Services.Jwt;
using BLL.Services.MailService;
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
    private readonly IAuthService _auth;
    private readonly JwtTokenService _jwt;
    private readonly DollDbContext _db;
    private readonly IConfiguration _cfg;
    private readonly SmtpEmailSender _email;

    public AuthController(
        IAuthService auth,
        JwtTokenService jwt,
        DollDbContext db,
        IConfiguration cfg,
        SmtpEmailSender email)
    {
        _auth = auth;
        _jwt = jwt;
        _db = db;
        _cfg = cfg;
        _email = email;
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
        if (req == null || !ModelState.IsValid)
            return BadRequest(new { message = "Invalid request body" });

        var user = await _auth.AuthenticateAsync(req.Username, req.Password);
        if (user == null)
            return Unauthorized(new { message = "Invalid username or password" });

        var minutes = int.TryParse(_cfg["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
        var accessToken = _jwt.CreateAccessToken(user, TimeSpan.FromMinutes(minutes));

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

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _db.Users.AnyAsync(u => u.UserName == req.Username))
            return BadRequest(new { message = "Username already exists" });

        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            return BadRequest(new { message = "Email already registered" });

        var user = await _auth.RegisterAsync(req.Username, req.Password, "customer", req.Email);

        return Ok(new
        {
            id = user.UserID,
            username = user.UserName,
            email = user.Email,
            role = user.Role,
            message = "Register successful"
        });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
    {
        var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(idClaim, out var userId))
            return Unauthorized();

        var ok = await _auth.ChangePasswordAsync(userId, req.CurrentPassword, req.NewPassword);
        return ok ? Ok(new { message = "Password changed successfully" })
                  : BadRequest(new { message = "Invalid current password" });
    }
}
