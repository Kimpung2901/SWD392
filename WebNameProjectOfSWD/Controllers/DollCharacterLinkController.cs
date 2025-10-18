using BLL.DTO.DollCharacterLinkDTO;
using BLL.IService;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DollCharacterLinkController : ControllerBase
{
    private readonly IDollCharacterLinkService _service;
    private readonly ILogger<DollCharacterLinkController> _logger;

    public DollCharacterLinkController(
        IDollCharacterLinkService service,
        ILogger<DollCharacterLinkController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Lấy tất cả links
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(new { message = "Lấy danh sách link thành công", data = result });
    }

    /// <summary>
    /// Lấy link theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null
            ? NotFound(new { message = $"Không tìm thấy link #{id}" })
            : Ok(new { message = "Lấy thông tin link thành công", data = result });
    }

    /// <summary>
    /// Lấy links theo OwnedDollID
    /// </summary>
    [HttpGet("owneddoll/{ownedDollId}")]
    public async Task<IActionResult> GetByOwnedDollId(int ownedDollId)
    {
        var result = await _service.GetByOwnedDollIdAsync(ownedDollId);
        return Ok(new { message = $"Lấy danh sách link của OwnedDoll #{ownedDollId} thành công", data = result });
    }

    /// <summary>
    /// Lấy links theo UserCharacterID
    /// </summary>
    [HttpGet("usercharacter/{userCharacterId}")]
    public async Task<IActionResult> GetByUserCharacterId(int userCharacterId)
    {
        var result = await _service.GetByUserCharacterIdAsync(userCharacterId);
        return Ok(new { message = $"Lấy danh sách link của UserCharacter #{userCharacterId} thành công", data = result });
    }

    /// <summary>
    /// Lấy các links đang active
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveLinks()
    {
        var result = await _service.GetActiveLinksAsync();
        return Ok(new { message = "Lấy danh sách link active thành công", data = result });
    }

    /// <summary>
    /// Bind character với doll (tạo mới link)
    /// </summary>
    [HttpPost("bind")]
    public async Task<IActionResult> Create([FromBody] CreateDollCharacterLinkDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.LinkID },
                new { message = "Bind character với doll thành công", data = created }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi bind character với doll");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật thông tin link
    /// </summary>
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateDollCharacterLinkDto dto)
    {
        _logger.LogInformation("PATCH Request - ID: {Id}, DTO: {@Dto}", id, dto);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _service.UpdatePartialAsync(id, dto);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy link #{id}" })
                : Ok(new { message = "Cập nhật link thành công", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật link #{Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Unbind character khỏi doll
    /// </summary>
    [HttpPost("{id}/unbind")]
    public async Task<IActionResult> Unbind(int id)
    {
        try
        {
            var result = await _service.UnbindAsync(id);
            return result
                ? Ok(new { message = "Unbind character khỏi doll thành công" })
                : NotFound(new { message = $"Không tìm thấy link #{id}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi unbind link #{Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa link
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result
            ? Ok(new { message = "Xóa link thành công" })
            : NotFound(new { message = $"Không tìm thấy link #{id}" });
    }
}