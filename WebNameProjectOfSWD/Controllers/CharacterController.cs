using BLL.DTO.CharacterDTO;
using BLL.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers
{
[ApiController]
[Route("api/characters")]
[Authorize]
public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _service;
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(ICharacterService service, ILogger<CharacterController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCharacters(
            [FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDir,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET /api/Character failed");
                return StatusCode(500, new { message = ex.Message, detail = ex.ToString() });
            }
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCharacterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.CharacterId },
                    new { message = "Tạo character thành công", data = created }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateCharacterDto dto)
        {
            _logger.LogInformation("PATCH Request - ID: {Id}, DTO: {@Dto}", id, dto);
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdatePartialAsync(id, dto);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy character #{id}" })
                : Ok(new { message = "Cập nhật thành công", data = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new { message = "Xóa thành công" })
                : NotFound();
        }
    }
}
