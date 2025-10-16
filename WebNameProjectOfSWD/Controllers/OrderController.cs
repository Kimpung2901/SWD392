using BLL.DTO.OrderDTO;
using BLL.IService;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService service, ILogger<OrderController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new { message = "Lấy danh sách đơn hàng thành công", data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy đơn hàng #{id}" })
                : Ok(new { message = "Lấy thông tin đơn hàng thành công", data = result });
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetByIdWithItems(int id)
        {
            var result = await _service.GetByIdWithItemsAsync(id);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy đơn hàng #{id}" })
                : Ok(new { message = "Lấy chi tiết đơn hàng thành công", data = result });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _service.GetByUserIdAsync(userId);
            return Ok(new { message = $"Lấy đơn hàng của user #{userId} thành công", data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.OrderID },
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
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateOrderDto dto)
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

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _service.CancelOrderAsync(id);
                return result
                    ? Ok(new { message = "Đã hủy đơn hàng thành công" })
                    : NotFound(new { message = $"Không tìm thấy đơn hàng #{id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hủy đơn hàng #{Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new { message = "Đã xóa đơn hàng thành công" })
                : NotFound(new { message = $"Không tìm thấy đơn hàng #{id}" });
        }
    }
}