using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using BLL.DTO;
using BLL.DTO.UserDto;
using BLL.Services.UsersService;
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
    private readonly UserService _users;
    private readonly JwtTokenService _jwt;
    private readonly DollDbContext _db;
    private readonly IConfiguration _cfg;
    private readonly SmtpEmailSender _email;

    public AuthController(UserService users, JwtTokenService jwt, DollDbContext db, IConfiguration cfg, SmtpEmailSender email)
    {
        _users = users ?? throw new ArgumentNullException(nameof(users));
        _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _cfg = cfg ?? throw new ArgumentNullException(nameof(cfg));
        _email = email ?? throw new ArgumentNullException(nameof(email));
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
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.RefreshToken))
            return BadRequest(new { message = "refreshToken missing" });

        var token = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == req.RefreshToken);

        if (token == null || token.Revoked != null || token.Expires <= DateTime.UtcNow)
            return Unauthorized(new { message = "Refresh token is invalid or expired" });

        var minutes = int.TryParse(_cfg["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
        var newAccess = _jwt.CreateAccessToken(token.User, TimeSpan.FromMinutes(minutes));

        return Ok(new { accessToken = newAccess, expiresAt = DateTime.UtcNow.AddMinutes(minutes) });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.RefreshToken))
            return BadRequest(new { message = "refreshToken missing!!" });

        var token = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == req.RefreshToken);
        if (token == null) return NotFound(new { message = "Refresh token is not exist!" });

        token.Revoked = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { message = "revoked refresh token" });
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

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email))
                return BadRequest(new { message = "Email is required" });

            // tìm user (ẩn thtin tồn tại email )
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user == null)
                return Ok(new { message = "OTP sent if email exists" });

            // tạo mã OTP
            var code = new Random().Next(100000, 999999).ToString();

            var pr = new PasswordReset
            {
                UserID = user.UserID,
                Code = code,
                Expires = DateTime.UtcNow.AddMinutes(10),
                Created = DateTime.UtcNow,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                Used = false
            };
            _db.PasswordResets.Add(pr);
            await _db.SaveChangesAsync();

            var html = $@"
            <p>Xin chào {user.UserName},</p>
            <p>Mã OTP đặt lại mật khẩu của bạn là: <b>{code}</b></p>
            <p>OTP sẽ hết hạn sau 10 phút.</p>";

            try
            {
                await _email.SendAsync(user.Email, "[SWD392] OTP đặt lại mật khẩu", html, "Doll Store");
                return Ok(new { message = "OTP sent if email exists" });
            }
            catch (SmtpException smtpEx)
            {
                return StatusCode(500, new { message = "Send email failed (SMTP)", detail = smtpEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Send email failed", detail = ex.Message });
            }
        }

        [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email) ||
            string.IsNullOrWhiteSpace(req.Otp) ||
            string.IsNullOrWhiteSpace(req.NewPassword))
            return BadRequest(new { message = "Invalid input" });

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user == null) return BadRequest(new { message = "Invalid OTP" });

        var rec = await _db.PasswordResets
            .Where(x => x.UserID == user.UserID && !x.Used)
            .OrderByDescending(x => x.Created)
            .FirstOrDefaultAsync();

        if (rec == null || rec.Code != req.Otp || rec.Expires <= DateTime.UtcNow)
            return BadRequest(new { message = "OTP is invalid or expired" });

        // Hash mật khẩu mới (BCrypt)
        var hash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        user.Password = hash;


        rec.Used = true;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Password reset successful" });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var username = req.Username.Trim();
        var email = req.Email.Trim().ToLowerInvariant();

        // Kiểm tra trùng
        if (await _db.Users.AnyAsync(u => u.UserName == username))
            return BadRequest(new { message = "Username already exists" });

        if (await _db.Users.AnyAsync(u => u.Email.ToLower() == email))
            return BadRequest(new { message = "Email already registered" });

        // Hash mật khẩu
        var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);

        var now = DateTime.UtcNow;

        var user = new User
        {
            UserName = req.Username,
            Email = req.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Status = "Active",
            Role = "customer",                
            CreatedAt = DateTime.UtcNow
        };
        _db.Users.Add(user);
       
        try
        {
            await _db.SaveChangesAsync();
            return Ok(new
            {
                id = user.UserID,
                username = user.UserName,
                email = user.Email,
                role = user.Role,
                status = user.Status,
                message = "Register successful"
            });
        }
        catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sql)
        {
            if (sql.Message.Contains("CHECK constraint") && sql.Message.Contains("Role"))
                return BadRequest(new { message = "Role value violates CHECK constraint (allowed: admin/manager/Customer)" });

            if (sql.Message.Contains("IX_") || sql.Message.Contains("UNIQUE"))
                return BadRequest(new { message = "Username/Email already exists (unique constraint)" });

            return StatusCode(500, new { message = "Save failed", detail = sql.Message });
        }
    }







}
