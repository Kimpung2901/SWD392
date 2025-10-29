using BLL.DTO.DollModelDTO;
using BLL.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/doll-models")]
    [Authorize]
    public class DollModelController : ControllerBase
    {
        private readonly IDollModelService _svc;
        private readonly ILogger<DollModelController> _logger;

        public DollModelController(IDollModelService svc, ILogger<DollModelController> logger)
        {
            _svc = svc;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetDollModels(
            [FromQuery] int? dollTypeId,
            [FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDir,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _svc.GetAsync(dollTypeId, search, sortBy, sortDir, page, pageSize);
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

        [HttpGet("doll-type-id/{dollTypeId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByDollTypeId(int dollTypeId)
        {
            var models = await _svc.GetByDollTypeIdAsync(dollTypeId);
            return Ok(models);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _svc.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDollModelDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _svc.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.DollModelID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doll model");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateDollModelDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _svc.UpdatePartialAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _svc.SoftDeleteAsync(id);
            return result
                ? NoContent()
                : NotFound();
        }

        [HttpDelete("{id:int}/hard")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var result = await _svc.HardDeleteAsync(id);
            return result
                ? NoContent()
                : NotFound();
        }
    }
}
