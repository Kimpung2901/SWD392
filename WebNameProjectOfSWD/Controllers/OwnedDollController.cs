using BLL.DTO.OwnedDollDTO;
using BLL.IService;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnedDollController : ControllerBase
    {
        private readonly IOwnedDollService _service;
        private readonly ILogger<OwnedDollController> _logger;

        public OwnedDollController(
            IOwnedDollService service,
            ILogger<OwnedDollController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new { message = "Lấy danh sách owned doll thành công", data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy owned doll #{id}" })
                : Ok(new { message = "Lấy thông tin owned doll thành công", data = result });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _service.GetByUserIdAsync(userId);
            return Ok(new { message = $"Lấy danh sách owned doll của user #{userId} thành công", data = result });
        }

        [HttpGet("doll-variant/{dollVariantId}")]
        public async Task<IActionResult> GetByDollVariantId(int dollVariantId)
        {
            var result = await _service.GetByDollVariantIdAsync(dollVariantId);
            return Ok(new { message = $"Lấy danh sách owned doll của variant #{dollVariantId} thành công", data = result });
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
                    new { message = "Tạo owned doll thành công", data = created }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo owned doll");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateOwnedDollDto dto)
        {
            _logger.LogInformation("PATCH Request - ID: {Id}, DTO: {@Dto}", id, dto);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdatePartialAsync(id, dto);
                return result == null
                    ? NotFound(new { message = $"Không tìm thấy owned doll #{id}" })
                    : Ok(new { message = "Cập nhật owned doll thành công", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật owned doll #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _service.SoftDeleteAsync(id);
            return result
                ? Ok(new { message = "Đã xóa mềm owned doll thành công" })
                : NotFound(new { message = $"Không tìm thấy owned doll #{id}" });
        }

        [HttpDelete("hard/{id}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var result = await _service.HardDeleteAsync(id);
            return result
                ? Ok(new { message = "Đã xóa vĩnh viễn owned doll thành công" })
                : NotFound(new { message = $"Không tìm thấy owned doll #{id}" });
        }
    }
}