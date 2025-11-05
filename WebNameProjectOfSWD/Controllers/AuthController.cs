using System.ComponentModel.DataAnnotations;
using BLL.DTO;
using BLL.IService;
using BLL.Services.Jwt;
using DAL.Enum;
using DAL.Models;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IUserService _userService;
    private readonly JwtTokenService _jwt;
    private readonly IConfiguration _cfg;
    private readonly IOtpService _otp;

    public AuthController(
        IAuthService auth,
        IUserService userService,
        JwtTokenService jwt,
        IConfiguration cfg,
        IOtpService otp)
    {
        _auth = auth;
        _userService = userService;
        _jwt = jwt;
        _cfg = cfg;
        _otp = otp;
    }

    public class LoginRequest
    {
        [Required] public string Username { get; set; } = null!;
        [Required] public string Password { get; set; } = null!;
        public string? DeviceToken { get; set; } 
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

  
        if (!string.IsNullOrWhiteSpace(req.DeviceToken))
        {
            await _userService.UpdateDeviceTokenAsync(user.UserID, req.DeviceToken);
        }

        var minutes = int.TryParse(_cfg["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
        var accessToken = _jwt.CreateAccessToken(user, TimeSpan.FromMinutes(minutes));

        // Tạo refresh token qua service
        var refreshToken = await _userService.CreateRefreshTokenAsync(
            user.UserID,
            HttpContext.Connection.RemoteIpAddress?.ToString()
        );

        return Ok(new
        {
            accessToken,
            expiresAt = DateTime.UtcNow.AddMinutes(minutes),
            refreshToken = refreshToken.Token,
            userId = user.UserID,
            username = user.UserName,
            fullName = user.FullName,
            role = user.Role
        });

    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

   
        if (await _userService.CheckUserExistsAsync(req.Username, req.Email))
            return BadRequest(new { message = "Username or email already exists" });

        var user = await _auth.RegisterAsync(
            req.Username, 
            req.Password, 
            "customer", 
            req.Email, 
            req.Phones,
            req.Age,
            req.FullName);

        return Ok(new
        {
            id = user.UserID,
            username = user.UserName,
            email = user.Email,
            phones = user.Phones,
            age = user.Age, 
            fullName = user.FullName,
            role = user.Role,
            message = "Register successful"
        });
    }

    [HttpPost("change-password")]
    [Authorize(Roles = "customer")]
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
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> ForgotPasswordOtp([FromBody] ForgotPasswordOtpRequest req)
    {

        var user = await _userService.GetUserByEmailAsync(req.Email.Trim());
        if (user != null && !user.IsDeleted && user.Status == UserStatus.Active)
        {
            await _otp.SendOtpAsync(req.Email.Trim());
        }
        return Ok(new { message = "If the email exists, an OTP has been sent." });
    }

    [HttpPost("reset-password-otp")]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> ResetPasswordOtp([FromBody] ResetPasswordOtpRequest req)
    {
        // validate OTP
        var ok = await _otp.VerifyOtpAsync(req.Email.Trim(), req.Otp);
        if (!ok) return BadRequest(new { message = "Invalid or expired OTP" });


        var result = await _userService.ResetPasswordAsync(req.Email.Trim(), req.NewPassword);
        if (!result)
            return BadRequest(new { message = "Failed to reset password" });

        return Ok(new { message = "Password reset successful" });
    }

    [HttpPost("google-login")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest req)
    {
        try
        {
            // Kiểm tra Firebase có được khởi tạo không
            if (FirebaseApp.DefaultInstance == null)
            {
                return StatusCode(503, new { message = "Google Login is not configured. Please contact administrator." });
            }

            // Xác minh token từ Firebase
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(req.IdToken);
            string uid = decodedToken.Uid;
            string email = decodedToken.Claims.TryGetValue("email", out var emailObj) ? emailObj?.ToString() : null;
            string name = decodedToken.Claims.TryGetValue("name", out var nameObj) ? nameObj?.ToString() : null;

            if (string.IsNullOrEmpty(email))
                return BadRequest(new { message = "Email not found in Google token" });


            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                // Tạo user mới qua RegisterAsync
                user = await _auth.RegisterAsync(
                    username: name ?? email.Split('@')[0],
                    rawPassword: Guid.NewGuid().ToString(), // Password ngẫu nhiên cho Google login
                    role: "customer",
                    email: email,
                    phone: null,
                    age: null,
                    fullName: name
                );
            }
            else if (user.IsDeleted || !string.Equals(user.Status.ToString(), "active", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Account is inactive or deleted" });
            }


            if (!string.IsNullOrWhiteSpace(req.DeviceToken))
            {
                await _userService.UpdateDeviceTokenAsync(user.UserID, req.DeviceToken);
            }

            var minutes = int.TryParse(_cfg["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
            var jwt = _jwt.CreateAccessToken(user, TimeSpan.FromMinutes(minutes));


            var refreshToken = await _userService.CreateRefreshTokenAsync(
                user.UserID,
                HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new
            {
                message = "Login success",
                accessToken = jwt,
                expiresAt = DateTime.UtcNow.AddMinutes(minutes),
                refreshToken = refreshToken.Token,
                userId = user.UserID,
                username = user.UserName,
                role = user.Role,
                user = new
                {
                    id = user.UserID,
                    username = user.UserName,
                    fullName = user.FullName,
                    email = user.Email,
                    role = user.Role
                }
            });
        }
        catch (FirebaseAuthException ex)
        {
            return Unauthorized(new { message = "Invalid Google token", error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(503, new { message = "Firebase service not available", error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Server error", error = ex.Message });
        }
    }

    public class UpdateDeviceTokenRequest
    {
        [Required]
        [MaxLength(500)]
        public string DeviceToken { get; set; } = null!;
    }

    [HttpPut("device-token")]
    [Authorize] 
    public async Task<IActionResult> UpdateDeviceToken([FromBody] UpdateDeviceTokenRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(idClaim, out var userId))
            return Unauthorized();

        var success = await _userService.UpdateDeviceTokenAsync(userId, req.DeviceToken);
        
        return success 
            ? Ok(new { message = "Device token updated successfully" })
            : NotFound(new { message = "User not found" });
    }
}

