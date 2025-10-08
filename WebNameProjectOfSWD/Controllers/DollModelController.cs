using BLL.DTO.DollModelDTO;
using BLL.IService;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DollModelController : ControllerBase
{
    private readonly IDollModelService _svc;
    public DollModelController(IDollModelService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
        => (await _svc.GetByIdAsync(id)) is { } m ? Ok(m) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create(CreateDollModelDto dto)
        => Ok(await _svc.CreateAsync(dto));

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDollModelDto dto)
        => (await _svc.UpdatePartialAsync(id, dto)) is { } m ? Ok(m) : NotFound();

    [HttpDelete("soft/{id}")]
    public async Task<IActionResult> SoftDelete(int id)
        => await _svc.SoftDeleteAsync(id) ? Ok("Soft deleted") : NotFound();

    [HttpDelete("hard/{id}")]
    public async Task<IActionResult> HardDelete(int id)
        => await _svc.HardDeleteAsync(id) ? Ok("Hard deleted") : NotFound();
}
