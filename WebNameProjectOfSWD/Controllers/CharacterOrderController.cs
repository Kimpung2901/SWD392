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
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CharacterOrderRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.CharacterOrderID },
                    new { message = "CharacterOrder created successfully", data = created }
                );
            }
            catch (Exception ex)
            {
                // Log chi ti?t l?i
                _logger.LogError(ex, "Error creating CharacterOrder");
                
                // Tr? v? chi ti?t l?i bao g?m inner exception
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new { 
                    message = ex.Message,
                    innerMessage = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CharacterOrderRequest dto)
        {
            _logger.LogInformation("PUT Request - ID: {Id}, DTO: {@Dto}", id, dto);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, dto);
            return result == null
                ? NotFound(new { message = $"CharacterOrder #{id} not found" })
                : Ok(new { message = "Updated successfully", data = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new { message = "Deleted successfully" })
                : NotFound();
        }
    }
}