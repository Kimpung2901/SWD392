using Microsoft.AspNetCore.Mvc;
using BLL.DTO.UserCharacterDTO;
using BLL.Helper;
using BLL.IService;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/user-characters")]
    public class UserCharacterController : ControllerBase
    {
        private readonly IUserCharacterService _service;
        private readonly ILogger<UserCharacterController> _logger;

        public UserCharacterController(IUserCharacterService service, ILogger<UserCharacterController> logger)
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
            if (data == null)
            {
                // Guard: service returned null -> return empty result set instead of crashing
                return Ok(new
                {
                    message = "Lấy danh sách user character thành công",
                    items = new object[0],
                    pagination = BuildPagination(0, page, pageSize)
                });
            }

            var query = data.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLowerInvariant();
                query = query.Where(uc =>
                    uc.UserCharacterID.ToString().Contains(term) ||
                    (uc.UserName ?? string.Empty).ToLowerInvariant().Contains(term) ||
                    (uc.CharacterName ?? string.Empty).ToLowerInvariant().Contains(term) ||
                    (uc.PackageName ?? string.Empty).ToLowerInvariant().Contains(term) ||
                    (uc.StatusDisplay ?? string.Empty).ToLowerInvariant().Contains(term));
            }

            var total = query.Count();
            query = string.IsNullOrWhiteSpace(sortBy)
                ? query.OrderByDescending(uc => uc.CreatedAt)
                : query.ApplySort(sortBy, sortDir);
            query = query.ApplyPagination(page, pageSize);

            var items = query.ToList();

            return Ok(new
            {
                message = "L?y danh s�ch user character th�nh c�ng",
                items,
                pagination = BuildPagination(total, page, pageSize)
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Kh�ng t�m th?y user character #{id}" })
                : Ok(new { message = "L?y th�ng tin user character th�nh c�ng", data = result });
        }

        [HttpGet("users/{userId:int}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _service.GetByUserIdAsync(userId);
            return Ok(new { message = $"L?y danh s�ch character c?a user #{userId} th�nh c�ng", data = result });
        }

        [HttpGet("users/{userId:int}/active")]
        public async Task<IActionResult> GetActiveSubscriptions(int userId)
        {
            var result = await _service.GetActiveSubscriptionsAsync(userId);
            return Ok(new { message = $"L?y danh s�ch subscription dang active c?a user #{userId} th�nh c�ng", data = result });
        }

        [HttpGet("characters/{characterId:int}")]
        public async Task<IActionResult> GetByCharacterId(int characterId)
        {
            var result = await _service.GetByCharacterIdAsync(characterId);
            return Ok(new { message = $"L?y danh s�ch user c?a character #{characterId} th�nh c�ng", data = result });
        }

        [HttpGet("packages/{packageId:int}")]
        public async Task<IActionResult> GetByPackageId(int packageId)
        {
            var result = await _service.GetByPackageIdAsync(packageId);
            return Ok(new { message = $"L?y danh s�ch user c?a package #{packageId} th�nh c�ng", data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCharacterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                if (created == null)
                {
                    // Defensive: if service didn't throw but returned null, return BadRequest
                    return BadRequest(new { message = "Không thể tạo user character" });
                }
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.UserCharacterID },
                    new { message = "T?o user character th�nh c�ng", data = created }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi t?o user character");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateUserCharacterDto dto)
        {
            _logger.LogInformation("PATCH Request - ID: {Id}, DTO: {@Dto}", id, dto);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdatePartialAsync(id, dto);
                return result == null
                    ? NotFound(new { message = $"Kh�ng t�m th?y user character #{id}" })
                    : Ok(new { message = "C?p nh?t user character th�nh c�ng", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi c?p nh?t user character #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/renew")]
        public async Task<IActionResult> RenewSubscription(int id)
        {
            try
            {
                var result = await _service.RenewSubscriptionAsync(id);
                return result
                    ? Ok(new { message = "Gia h?n subscription th�nh c�ng" })
                    : NotFound(new { message = $"Kh�ng t�m th?y user character #{id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi gia h?n subscription #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new { message = "X�a user character th�nh c�ng" })
                : NotFound(new { message = $"Kh�ng t�m th?y user character #{id}" });
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
