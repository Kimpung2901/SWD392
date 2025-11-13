using BLL.DTO.UserDTO;
using BLL.IService;
using DAL.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/users")] 
[Authorize] 
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
[Authorize(Policy = "AdminOrManager")]
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
            message = "L?y danh s�ch users th�nh c�ng",
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

    [HttpGet("{id}")]
    //[Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("admin");

        if (!isAdmin && currentUserId != id)
            return Forbid();

        var user = await _svc.GetByIdAsync(id);
        return user == null
            ? NotFound(new { success = false, message = $"Kh�ng t�m th?y user #{id}" })
            : Ok(new { success = true, data = user });
    }


    [HttpPost]
    [Authorize(Policy = "AdminOrManager")]
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
                new { success = true, message = "T?o user th�nh c�ng", data = created }
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }


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
            return NotFound(new { success = false, message = $"Kh�ng t�m th?y user #{id}" });

        return Ok(new { success = true, message = "C?p nh?t th�nh c�ng", data = updated });
    }


    [HttpPatch("{id}/status")]
    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateUserStatusDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        var updated = await _svc.UpdateStatusAsync(id, dto.Status);
        if (updated == null)
            return NotFound(new { success = false, message = $"Kh�ng t?m th?y user #{id}" });

        return Ok(new { success = true, message = "C?p nh?t tr?ng th�i th?nh c?ng", data = updated });
    }


    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        await _svc.SoftDeleteAsync(id);
        return Ok(new { success = true, message = "�� x�a m?m user" });
    }


    [HttpDelete("{id}/permanent")]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> HardDelete(int id)
    {
        await _svc.HardDeleteAsync(id);
        return Ok(new { success = true, message = "�� x�a vinh vi?n user" });
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst("UserID")
                    ?? User.FindFirst("sub");

        return claim != null && int.TryParse(claim.Value, out var userId) ? userId : 0;
    }
}
