using BLL.DTO.DollCharacterLinkDTO;
using BLL.Helper;
using BLL.IService;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/doll-character-links")]
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
    /// L?y t?t c? links
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var data = await _service.GetAllAsync();
        var query = data.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(l =>
                (l.OwnedDollSerialCode ?? string.Empty).ToLowerInvariant().Contains(term) ||
                (l.CharacterName ?? string.Empty).ToLowerInvariant().Contains(term) ||
                l.StatusDisplay.ToLowerInvariant().Contains(term) ||
                (l.Note ?? string.Empty).ToLowerInvariant().Contains(term));
        }

        var total = query.Count();
        query = string.IsNullOrWhiteSpace(sortBy)
            ? query.OrderByDescending(l => l.BoundAt)
            : query.ApplySort(sortBy, sortDir);
        query = query.ApplyPagination(page, pageSize);
        var items = query.ToList();

        return Ok(new
        {
            message = "L?y danh s�ch link th�nh c�ng",
            items,
            pagination = BuildPagination(total, page, pageSize)
        });
    }

    /// <summary>
    /// L?y link theo ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null
            ? NotFound(new { message = $"Kh�ng t�m th?y link #{id}" })
            : Ok(new { message = "L?y th�ng tin link th�nh c�ng", data = result });
    }

    /// <summary>
    /// L?y links theo OwnedDollID
    /// </summary>
    [HttpGet("owned-dolls/{ownedDollId:int}")]
    public async Task<IActionResult> GetByOwnedDollId(int ownedDollId)
    {
        var result = await _service.GetByOwnedDollIdAsync(ownedDollId);
        return Ok(new { message = $"L?y danh s�ch link c?a OwnedDoll #{ownedDollId} th�nh c�ng", data = result });
    }

    /// <summary>
    /// L?y links theo UserCharacterID
    /// </summary>
    [HttpGet("user-characters/{userCharacterId:int}")]
    public async Task<IActionResult> GetByUserCharacterId(int userCharacterId)
    {
        var result = await _service.GetByUserCharacterIdAsync(userCharacterId);
        return Ok(new { message = $"L?y danh s�ch link c?a UserCharacter #{userCharacterId} th�nh c�ng", data = result });
    }

    /// <summary>
    /// L?y c�c links dang active
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveLinks()
    {
        var result = await _service.GetActiveLinksAsync();
        return Ok(new { message = "L?y danh s�ch link active th�nh c�ng", data = result });
    }

    /// <summary>
    /// Bind character v?i doll (t?o m?i link)
    /// </summary>
    [HttpPost]
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
                new { message = "Bind character v?i doll th�nh c�ng", data = created }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi bind character v?i doll");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// C?p nh?t th�ng tin link
    /// </summary>
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateDollCharacterLinkDto dto)
    {
        _logger.LogInformation("PATCH Request - ID: {Id}, DTO: {@Dto}", id, dto);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _service.UpdatePartialAsync(id, dto);
            return result == null
                ? NotFound(new { message = $"Kh�ng t�m th?y link #{id}" })
                : Ok(new { message = "C?p nh?t link th�nh c�ng", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi c?p nh?t link #{Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Unbind character kh?i doll
    /// </summary>
    [HttpPost("{id:int}/unbind")]
    public async Task<IActionResult> Unbind(int id)
    {
        try
        {
            var result = await _service.UnbindAsync(id);
            return result
                ? Ok(new { message = "Unbind character kh?i doll th�nh c�ng" })
                : NotFound(new { message = $"Kh�ng t�m th?y link #{id}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "L?i khi unbind link #{Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// X�a link
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result
            ? Ok(new { message = "X�a link th�nh c�ng" })
            : NotFound(new { message = $"Kh�ng t�m th?y link #{id}" });
    }

    private static object BuildPagination(int total, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new
        {
            page,
            pageSize,
            total,
            totalPages,
            hasPreviousPage = page > 1,
            hasNextPage = page < totalPages
        };
    }
}
