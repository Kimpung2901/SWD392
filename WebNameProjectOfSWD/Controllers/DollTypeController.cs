// WebNameProjectOfSWD/Controllers/DollTypeController.cs
using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using BLL.DTO.DollTypeDTO;

[ApiController]
[Route("api/[controller]")]
public class DollTypeController : ControllerBase
{
    private readonly DollTypeService _service;
    public DollTypeController(DollTypeService service) => _service = service;

    // GET ?includeDeleted=true
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false) =>
        Ok(await _service.GetAllAsync(includeDeleted));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id, [FromQuery] bool includeDeleted = false)
    {
        var result = await _service.GetByIdAsync(id, includeDeleted);
        return result == null ? NotFound(new { message = $"DollType #{id} not found" }) : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDollTypeDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = result.DollTypeID }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDollTypeDto req)
    {
        var updated = await _service.UpdateAsync(id, req.Name, req.Description, req.Image, req.IsActive);
        if (updated == null) return NotFound(new { message = $"DollType #{id} not found" });

        return Ok(new { message = "Updated successfully", data = updated });
    }

    /// Soft delete (mặc định)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSoft(int id)
    {
        var deleted = await _service.DeleteSoftAsync(id);
        if (deleted == null) return NotFound(new { message = $"DollType #{id} not found" });

        return Ok(new { message = "Soft deleted", data = deleted });
    }

    /// Restore soft-deleted
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        var restored = await _service.RestoreAsync(id);
        if (restored == null) return NotFound(new { message = $"DollType #{id} not found or not deleted" });

        return Ok(new { message = "Restored", data = restored });
    }

    /// Hard delete (cẩn thận!)
    [HttpDelete("{id}/hard")]
    public async Task<IActionResult> DeleteHard(int id)
    {
        var deleted = await _service.DeleteHardAsync(id);
        if (deleted == null) return NotFound(new { message = $"DollType #{id} not found" });

        return Ok(new { message = "Hard deleted", data = deleted });
    }
}
