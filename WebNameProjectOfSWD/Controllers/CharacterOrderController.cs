using BLL.DTO.CharacterOrderDTO;
using BLL.Helper;
using BLL.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/character-orders")]
    public class CharacterOrderController : ControllerBase
    {
        private readonly ICharacterOrderService _service;
        private readonly ILogger<CharacterOrderController> _logger;

        public CharacterOrderController(ICharacterOrderService service, ILogger<CharacterOrderController> logger)
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
                    o.CharacterOrderID.ToString().Contains(term) ||
                    (o.PackageName ?? string.Empty).ToLowerInvariant().Contains(term) ||
                    (o.CharacterName ?? string.Empty).ToLowerInvariant().Contains(term) ||
                    o.StatusDisplay.ToLowerInvariant().Contains(term));
            }

            var total = query.Count();
            query = string.IsNullOrWhiteSpace(sortBy)
                ? query.OrderByDescending(o => o.CreatedAt)
                : query.ApplySort(sortBy, sortDir);
            query = query.ApplyPagination(page, pageSize);

            var items = query.ToList();

            return Ok(new
            {
                message = "L?y danh s�ch character order th�nh c�ng",
                items,
                pagination = BuildPagination(total, page, pageSize)
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Kh�ng t�m th?y character order #{id}" })
                : Ok(new { message = "L?y th�ng tin character order th�nh c�ng", data = result });
        }

        [HttpGet("user-characters/{userCharacterId:int}")]
        public async Task<IActionResult> GetByUserCharacterId(int userCharacterId)
        {
            var result = await _service.GetByUserCharacterIdAsync(userCharacterId);
            return Ok(new { message = $"L?y danh s�ch order c?a user character #{userCharacterId} th�nh c�ng", data = result });
        }

        [HttpGet("characters/{characterId:int}")]
        public async Task<IActionResult> GetByCharacterId(int characterId)
        {
            var result = await _service.GetByCharacterIdAsync(characterId);
            return Ok(new { message = $"L?y danh s�ch order c?a character #{characterId} th�nh c�ng", data = result });
        }

        [HttpGet("packages/{packageId:int}")]
        public async Task<IActionResult> GetByPackageId(int packageId)
        {
            var result = await _service.GetByPackageIdAsync(packageId);
            return Ok(new { message = $"L?y danh s�ch order c?a package #{packageId} th�nh c�ng", data = result });
        }

        [HttpGet("status/pending")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var result = await _service.GetPendingOrdersAsync();
            return Ok(new { message = "L?y danh s�ch order dang pending th�nh c�ng", data = result });
        }

        [HttpPost]
        [Authorize] // Y�u c?u dang nh?p
        public async Task<IActionResult> Create([FromBody] CreateCharacterOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value
                    ?? User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Kh�ng th? x�c d?nh user t? token" });
                }

                var created = await _service.CreateAsync(dto, userId);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.CharacterOrderID },
                    new { message = "T?o character order th�nh c�ng", data = created }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi t?o character order");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateCharacterOrderDto dto)
        {
            _logger.LogInformation("PATCH Request - ID: {Id}, DTO: {@Dto}", id, dto);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdatePartialAsync(id, dto);
                return result == null
                    ? NotFound(new { message = $"Kh�ng t�m th?y character order #{id}" })
                    : Ok(new { message = "C?p nh?t character order th�nh c�ng", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi c?p nh?t character order #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/complete")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            try
            {
                var result = await _service.CompleteOrderAsync(id);
                return result
                    ? Ok(new { message = "Ho�n th�nh order th�nh c�ng" })
                    : NotFound(new { message = $"Kh�ng t�m th?y character order #{id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi ho�n th�nh order #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var result = await _service.CancelOrderAsync(id);
                return result
                    ? Ok(new { message = "H?y order th�nh c�ng" })
                    : NotFound(new { message = $"Kh�ng t�m th?y character order #{id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi h?y order #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new { message = "X�a character order th�nh c�ng" })
                : NotFound(new { message = $"Kh�ng t�m th?y character order #{id}" });
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
