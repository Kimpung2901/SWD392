using BLL.DTO.DollTypeDTO;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DollTypeController : ControllerBase
{
    private readonly IDollTypeService _service;
    public DollTypeController(IDollTypeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDollTypeDto dto)
        => Ok(await _service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDollTypeDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDelete(int id)
        => await _service.SoftDeleteAsync(id)
            ? Ok(new { message = "Soft deleted successfully" })
            : NotFound();

    //[HttpDelete("{id}/hard")]
    //public async Task<IActionResult> HardDelete(int id)
    //    => await _service.HardDeleteAsync(id)
    //        ? Ok(new { message = "Hard deleted successfully" })
    //        : NotFound();
}
