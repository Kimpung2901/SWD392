using Microsoft.AspNetCore.Mvc;
using BLL.DTO.UserCharacterDTO;
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
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new { message = "Lấy danh sách user character thành công", data = result });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy user character #{id}" })
                : Ok(new { message = "Lấy thông tin user character thành công", data = result });
        }

        [HttpGet("users/{userId:int}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _service.GetByUserIdAsync(userId);
            return Ok(new { message = $"Lấy danh sách character của user #{userId} thành công", data = result });
        }

        [HttpGet("users/{userId:int}/active")]
        public async Task<IActionResult> GetActiveSubscriptions(int userId)
        {
            var result = await _service.GetActiveSubscriptionsAsync(userId);
            return Ok(new { message = $"Lấy danh sách subscription đang active của user #{userId} thành công", data = result });
        }

        [HttpGet("characters/{characterId:int}")]
        public async Task<IActionResult> GetByCharacterId(int characterId)
        {
            var result = await _service.GetByCharacterIdAsync(characterId);
            return Ok(new { message = $"Lấy danh sách user của character #{characterId} thành công", data = result });
        }

        [HttpGet("packages/{packageId:int}")]
        public async Task<IActionResult> GetByPackageId(int packageId)
        {
            var result = await _service.GetByPackageIdAsync(packageId);
            return Ok(new { message = $"Lấy danh sách user của package #{packageId} thành công", data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCharacterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.UserCharacterID },
                    new { message = "Tạo user character thành công", data = created }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo user character");
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
                    ? NotFound(new { message = $"Không tìm thấy user character #{id}" })
                    : Ok(new { message = "Cập nhật user character thành công", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật user character #{Id}", id);
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
                    ? Ok(new { message = "Gia hạn subscription thành công" })
                    : NotFound(new { message = $"Không tìm thấy user character #{id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gia hạn subscription #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new { message = "Xóa user character thành công" })
                : NotFound(new { message = $"Không tìm thấy user character #{id}" });
        }
    }
}
