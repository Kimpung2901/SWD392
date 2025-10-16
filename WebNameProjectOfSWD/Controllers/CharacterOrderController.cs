using BLL.DTO.CharacterOrderDTO;
using BLL.IService;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterOrderController : ControllerBase
    {
        private readonly ICharacterOrderService _service;
        private readonly ILogger<CharacterOrderController> _logger;

        public CharacterOrderController(
            ICharacterOrderService service,
            ILogger<CharacterOrderController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new { message = "Lấy danh sách đơn hàng character thành công", data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy đơn hàng #{id}" })
                : Ok(new { message = "Lấy thông tin đơn hàng thành công", data = result });
        }

        [HttpGet("character/{characterId}")]
        public async Task<IActionResult> GetByCharacterId(int characterId)
        {
            var result = await _service.GetByCharacterIdAsync(characterId);
            return Ok(new { message = $"Lấy đơn hàng của character #{characterId} thành công", data = result });
        }

        [HttpGet("package/{packageId}")]
        public async Task<IActionResult> GetByPackageId(int packageId)
        {
            var result = await _service.GetByPackageIdAsync(packageId);
            return Ok(new { message = $"Lấy đơn hàng của package #{packageId} thành công", data = result });
        }

        [HttpGet("user-character/{userCharacterId}")]
        public async Task<IActionResult> GetByUserCharacterId(int userCharacterId)
        {
            var result = await _service.GetByUserCharacterIdAsync(userCharacterId);
            return Ok(new { message = $"Lấy đơn hàng của user character #{userCharacterId} thành công", data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCharacterOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.CharacterOrderID },
                    new { message = "Tạo đơn hàng thành công", data = created }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo đơn hàng");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateCharacterOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdatePartialAsync(id, dto);
                return result == null
                    ? NotFound(new { message = $"Không tìm thấy đơn hàng #{id}" })
                    : Ok(new { message = "Cập nhật đơn hàng thành công", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật đơn hàng #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _service.SoftDeleteAsync(id);
            return result
                ? Ok(new { message = "Đã xóa mềm đơn hàng thành công" })
                : NotFound(new { message = $"Không tìm thấy đơn hàng #{id}" });
        }

        [HttpDelete("hard/{id}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var result = await _service.HardDeleteAsync(id);
            return result
                ? Ok(new { message = "Đã xóa vĩnh viễn đơn hàng thành công" })
                : NotFound(new { message = $"Không tìm thấy đơn hàng #{id}" });
        }
    }
}