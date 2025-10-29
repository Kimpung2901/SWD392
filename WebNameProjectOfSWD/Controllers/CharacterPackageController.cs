using BLL.DTO.CharacterPackageDTO;
using BLL.Helper;
using BLL.IService;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/character-packages")]
    public class CharacterPackageController : ControllerBase
    {
        private readonly ICharacterPackageService _service;
        private readonly ILogger<CharacterPackageController> _logger;

        public CharacterPackageController(
            ICharacterPackageService service,
            ILogger<CharacterPackageController> logger)
        {
            _service = service;
            _logger = logger;
        }

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
                query = query.Where(p =>
                    p.Name.ToLowerInvariant().Contains(term) ||
                    (p.CharacterName ?? string.Empty).ToLowerInvariant().Contains(term) ||
                    p.StatusDisplay.ToLowerInvariant().Contains(term) ||
                    p.Billing_Cycle.ToLowerInvariant().Contains(term) ||
                    p.Description.ToLowerInvariant().Contains(term));
            }

            var total = query.Count();
            query = string.IsNullOrWhiteSpace(sortBy)
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.ApplySort(sortBy, sortDir);
            query = query.ApplyPagination(page, pageSize);

            var items = query.ToList();

            return Ok(new
            {
                message = "L?y danh s�ch package th�nh c�ng",
                items,
                pagination = BuildPagination(total, page, pageSize)
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Kh�ng t�m th?y package #{id}" })
                : Ok(new { message = "L?y th�ng tin package th�nh c�ng", data = result });
        }

        [HttpGet("characters/{characterId:int}")]
        public async Task<IActionResult> GetByCharacterId(int characterId)
        {
            var result = await _service.GetByCharacterIdAsync(characterId);
            return Ok(new { message = $"L?y danh s�ch package c?a character #{characterId} th�nh c�ng", data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCharacterPackageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.PackageId },
                    new { message = "T?o package th�nh c�ng", data = created }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi t?o package");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateCharacterPackageDto dto)
        {
            _logger.LogInformation("PATCH Request - ID: {Id}, DTO: {@Dto}", id, dto);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdatePartialAsync(id, dto);
                return result == null
                    ? NotFound(new { message = $"Kh�ng t�m th?y package #{id}" })
                    : Ok(new { message = "C?p nh?t package th�nh c�ng", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi c?p nh?t package #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _service.SoftDeleteAsync(id);
            return result
                ? Ok(new { message = "D� x�a m?m package th�nh c�ng" })
                : NotFound(new { message = $"Kh�ng t�m th?y package #{id}" });
        }

        [HttpDelete("{id:int}/hard")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var result = await _service.HardDeleteAsync(id);
            return result
                ? Ok(new { message = "D� x�a vinh vi?n package th�nh c�ng" })
                : NotFound(new { message = $"Kh�ng t�m th?y package #{id}" });
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
}
