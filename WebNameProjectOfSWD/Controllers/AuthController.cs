using System.ComponentModel.DataAnnotations;
using BLL.DTO;
using BLL.IService;
using BLL.Services.Jwt;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.IRepo;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly JwtTokenService _jwt;
    private readonly DollDbContext _db;
    private readonly IConfiguration _cfg;
    private readonly IEmailSender _email;
    private readonly IOtpService _otp;
    private readonly IUserRepository _users; 

    public AuthController(
        IAuthService auth, 
        JwtTokenService jwt, 
        DollDbContext db,
        IConfiguration cfg, 
        IEmailSender email, 
        IOtpService otp,
        IUserRepository users) 
    {
        _auth = auth;
        _jwt = jwt;
        _db = db;
        _cfg = cfg;
        _email = email;
        _otp = otp; 
        _users = users;
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
        {
            #if DEBUG
            var userExists = await _db.Users.AnyAsync(u => 
                (u.UserName == req.Username || u.Email == req.Username) && !u.IsDeleted);
            if (userExists)
                return Unauthorized(new { message = "Invalid password" });
            #endif

            return Unauthorized(new { message = "Invalid username or password" });
        }

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

        var user = await _auth.RegisterAsync(req.Username, req.Password, "customer", req.Email, req.Phones);

        return Ok(new
        {
            id = user.UserID,
            username = user.UserName,
            email = user.Email,
            phones = user.Phones,
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
    [HttpPost("forgot-password-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPasswordOtp([FromBody] ForgotPasswordOtpRequest req)
    {
        var user = await _users.GetUserByEmailAsync(req.Email.Trim());
        if (user != null && !user.IsDeleted && user.Status.Equals("active", StringComparison.OrdinalIgnoreCase))
        {
            await _otp.SendOtpAsync(req.Email.Trim());
        }
        return Ok(new { message = "If the email exists, an OTP has been sent." });
    }

    [HttpPost("reset-password-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPasswordOtp([FromBody] ResetPasswordOtpRequest req)
    {
        // validate OTP
        var ok = await _otp.VerifyOtpAsync(req.Email.Trim(), req.Otp);
        if (!ok) return BadRequest(new { message = "Invalid or expired OTP" });

        // đổi mật khẩu
        var user = await _users.GetUserByEmailAsync(req.Email.Trim());
        if (user == null || user.IsDeleted) return BadRequest(new { message = "Invalid user" });

        user.Password = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        try
        {
            await _users.UpdateAsync(user);
            return Ok(new { message = "Password reset successful" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
        }
    }



}
