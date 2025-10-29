using BLL.DTO.OwnedDollDTO;
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
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new { message = "Lấy danh sách búp bê sở hữu thành công", data = result });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy búp bê sở hữu #{id}" })
                : Ok(new { message = "Lấy thông tin búp bê sở hữu thành công", data = result });
        }

        [HttpGet("users/{userId:int}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _service.GetByUserIdAsync(userId);
            return Ok(new { message = $"Lấy danh sách búp bê của user #{userId} thành công", data = result });
        }

        [HttpGet("variants/{dollVariantId:int}")]
        public async Task<IActionResult> GetByDollVariantId(int dollVariantId)
        {
            var result = await _service.GetByDollVariantIdAsync(dollVariantId);
            return Ok(new { message = $"Lấy danh sách búp bê variant #{dollVariantId} thành công", data = result });
        }

        [HttpGet("serial-code/{serialCode}")]
        public async Task<IActionResult> GetBySerialCode(string serialCode)
        {
            var result = await _service.GetBySerialCodeAsync(serialCode);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy búp bê với SerialCode '{serialCode}'" })
                : Ok(new { message = "Lấy thông tin búp bê thành công", data = result });
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
                    new { message = "Tạo búp bê sở hữu thành công", data = created }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo búp bê sở hữu");
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
                    ? NotFound(new { message = $"Không tìm thấy búp bê sở hữu #{id}" })
                    : Ok(new { message = "Cập nhật búp bê sở hữu thành công", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật búp bê sở hữu #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new { message = "Xóa búp bê sở hữu thành công" })
                : NotFound(new { message = $"Không tìm thấy búp bê sở hữu #{id}" });
        }
    }
}
