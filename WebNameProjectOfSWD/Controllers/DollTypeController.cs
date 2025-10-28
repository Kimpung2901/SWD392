using BLL.DTO.DollTypeDTO;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/doll-types")]
public class DollTypeController : ControllerBase
{
    private readonly IDollTypeService _service;
    public DollTypeController(IDollTypeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetDollTypes(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAsync(search, sortBy, sortDir, page, pageSize);
        return Ok(new
        {
            items = result.Items,
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDollTypeDto dto)
        => Ok(await _service.CreateAsync(dto));

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateDollTypeDto dto)
    {
        var result = await _service.UpdatePartialAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> SoftDelete(int id)
        => await _service.SoftDeleteAsync(id)
            ? Ok(new { message = "Soft deleted successfully" })
            : NotFound();
}
