using BLL.DTO.DollVariantDTO;
using BLL.IService;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DollVariantController : ControllerBase
    {
        private readonly IDollVariantService _service;

        public DollVariantController(IDollVariantService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("by-model/{dollModelId}")]
        public async Task<IActionResult> GetByDollModelId(int dollModelId)
        {
            var result = await _service.GetByDollModelIdAsync(dollModelId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDollVariantDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.DollVariantID },
                    new { message = "Tạo variant thành công", data = created }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateDollVariantDto dto)
        {
            var result = await _service.UpdatePartialAsync(id, dto);
            return result == null 
                ? NotFound(new { message = $"Không tìm thấy variant #{id}" }) 
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