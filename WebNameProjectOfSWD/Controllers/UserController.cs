using System.IdentityModel.Tokens.Jwt;
using BLL.DTO.UserDTO;
using BLL.Helper;
using BLL.IService;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _svc;
    private readonly IConfiguration _config;

    public UserController(IUserService svc, IConfiguration config)
    {
        _svc = svc;
        _config = config;
    }

    [HttpGet]
    //[Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id}")]
    //[Authorize(Roles = "admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _svc.GetByIdAsync(id);
        return user == null ? NotFound(new { message = $"Không tìm thấy user #{id}" }) : Ok(user);
    }

    [HttpPost]
    //[Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById), 
                new { id = created.UserID }, 
                new { message = "Tạo user thành công", data = created }
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    //[Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {

        var claim = User.FindFirst("UserID")
                 ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                 ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

        if (claim == null)
            return Unauthorized(new { message = "Token không có UserID" });

        if (!int.TryParse(claim.Value, out var currentUserId))
            return Unauthorized(new { message = "UserID không hợp lệ" });

        var currentRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "user";

        if (currentUserId != id && !string.Equals(currentRole, "admin", StringComparison.OrdinalIgnoreCase))
            return Forbid("Bạn không có quyền chỉnh người khác.");

        var updated = await _svc.UpdatePartialAsync(id, dto);
        if (updated == null)
            return NotFound(new { message = $"Không tìm thấy user #{id}" });

        return Ok(new { message = "Cập nhật thành công", data = updated });
    }

    [HttpDelete("soft/{id}")]
   //[Authorize(Roles = "admin")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        await _svc.SoftDeleteAsync(id);
        return Ok(new { message = "Đã xóa mềm user" });
    }

    [HttpDelete("hard/{id}")]
    //[Authorize(Roles = "admin")]
    public async Task<IActionResult> HardDelete(int id)
    {
        await _svc.HardDeleteAsync(id);
        return Ok(new { message = "Đã xóa vĩnh viễn user" });
    }
}
