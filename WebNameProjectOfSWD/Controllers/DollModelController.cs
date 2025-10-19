using BLL.DTO.DollModelDTO;
using BLL.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DollModelController : ControllerBase
    {
        private readonly IDollModelService _svc;
        private readonly ILogger<DollModelController> _logger;

        public DollModelController(IDollModelService svc, ILogger<DollModelController> logger)
        {
            _svc = svc;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _svc.GetAllAsync();
            return Ok(new { message = "Lấy danh sách doll model thành công", data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _svc.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy doll model #{id}" })
                : Ok(new { message = "Lấy thông tin doll model thành công", data = result });
        }

        [HttpGet("by-type/{dollTypeId}")]
        public async Task<IActionResult> GetByDollTypeId(int dollTypeId)
        {
            var result = await _svc.GetByDollTypeIdAsync(dollTypeId);
            return Ok(new { message = $"Lấy danh sách doll model của type #{dollTypeId} thành công", data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDollModelDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _svc.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.DollModelID },
                    new { message = "Tạo doll model thành công", data = created }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo doll model");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateDollModelDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _svc.UpdatePartialAsync(id, dto);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy doll model #{id}" })
                : Ok(new { message = "Cập nhật doll model thành công", data = result });
        }

        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _svc.SoftDeleteAsync(id);
            return result
                ? Ok(new { message = "Xóa mềm doll model thành công" })
                : NotFound(new { message = $"Không tìm thấy doll model #{id}" });
        }

        [HttpDelete("hard/{id}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var result = await _svc.HardDeleteAsync(id);
            return result
                ? Ok(new { message = "Xóa cứng doll model thành công" })
                : NotFound(new { message = $"Không tìm thấy doll model #{id}" });
        }
    }
}
