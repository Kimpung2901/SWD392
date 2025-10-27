using BLL.DTO.UserDTO;
using BLL.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/users")] // ✅ RESTful route
[Authorize] // ✅ Yêu cầu authentication
public class UserController : ControllerBase
{
    private readonly IUserService _svc;
    private readonly IConfiguration _config;

    public UserController(IUserService svc, IConfiguration config)
    {
        _svc = svc;
        _config = config;
    }

    /// <summary>
    /// Lấy danh sách users với search/sort/pagination
    /// GET /api/users?search=john&sortBy=userName&sortDir=asc&page=1&pageSize=10
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")] // ✅ Chỉ Admin
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _svc.GetAsync(search, sortBy, sortDir, page, pageSize);

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách users thành công",
            data = result.Items,
            pagination = new
            {
                result.Page,
                result.PageSize,
                result.Total,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            }
        });
    }

    /// <summary>
    /// Lấy thông tin user theo ID
    /// GET /api/users/{id}
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("admin");

        if (!isAdmin && currentUserId != id)
            return Forbid();

        var user = await _svc.GetByIdAsync(id);
        return user == null
            ? NotFound(new { success = false, message = $"Không tìm thấy user #{id}" })
            : Ok(new { success = true, data = user });
    }

    /// <summary>
    /// Tạo user mới
    /// POST /api/users
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        try
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.UserID },
                new { success = true, message = "Tạo user thành công", data = created }
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật thông tin user
    /// PATCH /api/users/{id}
    /// </summary>
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("admin");

        if (!isAdmin && currentUserId != id)
            return Forbid();

        var updated = await _svc.UpdatePartialAsync(id, dto);
        if (updated == null)
            return NotFound(new { success = false, message = $"Không tìm thấy user #{id}" });

        return Ok(new { success = true, message = "Cập nhật thành công", data = updated });
    }

    /// <summary>
    /// Soft delete user
    /// DELETE /api/users/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        await _svc.SoftDeleteAsync(id);
        return Ok(new { success = true, message = "Đã xóa mềm user" });
    }

    /// <summary>
    /// Hard delete user (permanent)
    /// DELETE /api/users/{id}/permanent
    /// </summary>
    [HttpDelete("{id}/permanent")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> HardDelete(int id)
    {
        await _svc.HardDeleteAsync(id);
        return Ok(new { success = true, message = "Đã xóa vĩnh viễn user" });
    }

    // ✅ Helper method
    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst("UserID")
                    ?? User.FindFirst("sub");

        return claim != null && int.TryParse(claim.Value, out var userId) ? userId : 0;
    }
}
