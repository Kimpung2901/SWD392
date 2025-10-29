using BLL.DTO.CharacterPackageDTO;
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
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new { message = "Lấy danh sách package thành công", data = result });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null 
                ? NotFound(new { message = $"Không tìm thấy package #{id}" })
                : Ok(new { message = "Lấy thông tin package thành công", data = result });
        }

        [HttpGet("characters/{characterId:int}")]
        public async Task<IActionResult> GetByCharacterId(int characterId)
        {
            var result = await _service.GetByCharacterIdAsync(characterId);
            return Ok(new { message = $"Lấy danh sách package của character #{characterId} thành công", data = result });
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
                    new { message = "Tạo package thành công", data = created }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo package");
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
                    ? NotFound(new { message = $"Không tìm thấy package #{id}" })
                    : Ok(new { message = "Cập nhật package thành công", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật package #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _service.SoftDeleteAsync(id);
            return result
                ? Ok(new { message = "Đã xóa mềm package thành công" })
                : NotFound(new { message = $"Không tìm thấy package #{id}" });
        }

        [HttpDelete("{id:int}/hard")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var result = await _service.HardDeleteAsync(id);
            return result
                ? Ok(new { message = "Đã xóa vĩnh viễn package thành công" })
                : NotFound(new { message = $"Không tìm thấy package #{id}" });
        }
    }
}
