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

        public CharacterOrderController(ICharacterOrderService service, ILogger<CharacterOrderController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new { message = "Lấy danh sách character order thành công", data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy character order #{id}" })
                : Ok(new { message = "Lấy thông tin character order thành công", data = result });
        }

        [HttpGet("usercharacter/{userCharacterId}")]
        public async Task<IActionResult> GetByUserCharacterId(int userCharacterId)
        {
            var result = await _service.GetByUserCharacterIdAsync(userCharacterId);
            return Ok(new { message = $"Lấy danh sách order của user character #{userCharacterId} thành công", data = result });
        }

        [HttpGet("character/{characterId}")]
        public async Task<IActionResult> GetByCharacterId(int characterId)
        {
            var result = await _service.GetByCharacterIdAsync(characterId);
            return Ok(new { message = $"Lấy danh sách order của character #{characterId} thành công", data = result });
        }

        [HttpGet("package/{packageId}")]
        public async Task<IActionResult> GetByPackageId(int packageId)
        {
            var result = await _service.GetByPackageIdAsync(packageId);
            return Ok(new { message = $"Lấy danh sách order của package #{packageId} thành công", data = result });
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var result = await _service.GetPendingOrdersAsync();
            return Ok(new { message = "Lấy danh sách order đang pending thành công", data = result });
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
                    new { message = "Tạo character order thành công", data = created }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo character order");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateCharacterOrderDto dto)
        {
            _logger.LogInformation("PATCH Request - ID: {Id}, DTO: {@Dto}", id, dto);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdatePartialAsync(id, dto);
                return result == null
                    ? NotFound(new { message = $"Không tìm thấy character order #{id}" })
                    : Ok(new { message = "Cập nhật character order thành công", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật character order #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            try
            {
                var result = await _service.CompleteOrderAsync(id);
                return result
                    ? Ok(new { message = "Hoàn thành order thành công" })
                    : NotFound(new { message = $"Không tìm thấy character order #{id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hoàn thành order #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var result = await _service.CancelOrderAsync(id);
                return result
                    ? Ok(new { message = "Hủy order thành công" })
                    : NotFound(new { message = $"Không tìm thấy character order #{id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hủy order #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new { message = "Xóa character order thành công" })
                : NotFound(new { message = $"Không tìm thấy character order #{id}" });
        }
    }
}