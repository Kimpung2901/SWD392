using BLL.DTO.OwnedDollDTO;
using BLL.Helper;
using BLL.IService;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/owned-dolls")]
    public class OwnedDollController : ControllerBase
    {
        private readonly IOwnedDollService _service;
        private readonly ILogger<OwnedDollController> _logger;

        public OwnedDollController(IOwnedDollService service, ILogger<OwnedDollController> logger)
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
                query = query.Where(o =>
                    o.SerialCode.ToLowerInvariant().Contains(term) ||
                    (o.UserName ?? string.Empty).ToLowerInvariant().Contains(term) ||
                    (o.DollVariantName ?? string.Empty).ToLowerInvariant().Contains(term) ||
                    o.StatusDisplay.ToLowerInvariant().Contains(term));
            }

        var total = query.Count();
        query = string.IsNullOrWhiteSpace(sortBy)
            ? query.OrderByDescending(o => o.Acquired_at)
            : query.ApplySort(sortBy, sortDir);
            query = query.ApplyPagination(page, pageSize);

            var items = query.ToList();

            return Ok(new
            {
                message = "Retrieved owned dolls successfully",
                items,
                pagination = BuildPagination(total, page, pageSize)
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Kh�ng t�m th?y b�p b� s? h?u #{id}" })
                : Ok(new { message = "L?y th�ng tin b�p b� s? h?u th�nh c�ng", data = result });
        }

        [HttpGet("users/{userId:int}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _service.GetByUserIdAsync(userId);
            return Ok(new { message = $"L?y danh s�ch b�p b� c?a user #{userId} th�nh c�ng", data = result });
        }

        [HttpGet("variants/{dollVariantId:int}")]
        public async Task<IActionResult> GetByDollVariantId(int dollVariantId)
        {
            var result = await _service.GetByDollVariantIdAsync(dollVariantId);
            return Ok(new { message = $"L?y danh s�ch b�p b� variant #{dollVariantId} th�nh c�ng", data = result });
        }

        [HttpGet("serial-code/{serialCode}")]
        public async Task<IActionResult> GetBySerialCode(string serialCode)
        {
            var result = await _service.GetBySerialCodeAsync(serialCode);
            return result == null
                ? NotFound(new { message = $"Kh�ng t�m th?y b�p b� v?i SerialCode '{serialCode}'" })
                : Ok(new { message = "L?y th�ng tin b�p b� th�nh c�ng", data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOwnedDollDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.OwnedDollID },
                    new { message = "T?o b�p b� s? h?u th�nh c�ng", data = created }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi t?o b�p b� s? h?u");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateOwnedDollDto dto)
        {
            _logger.LogInformation("PATCH Request - ID: {Id}, DTO: {@Dto}", id, dto);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdatePartialAsync(id, dto);
                return result == null
                    ? NotFound(new { message = $"Kh�ng t�m th?y b�p b� s? h?u #{id}" })
                    : Ok(new { message = "C?p nh?t b�p b� s? h?u th�nh c�ng", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi c?p nh?t b�p b� s? h?u #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new { message = "X�a b�p b� s? h?u th�nh c�ng" })
                : NotFound(new { message = $"Kh�ng t�m th?y b�p b� s? h?u #{id}" });
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
