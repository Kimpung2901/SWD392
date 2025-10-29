using BLL.DTO.OrderDTO;
using BLL.IService;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/order-items")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _service;
        private readonly ILogger<OrderItemController> _logger;

        public OrderItemController(IOrderItemService service, ILogger<OrderItemController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new { message = "Lấy danh sách order items thành công", data = result });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy order item #{id}" })
                : Ok(new { message = "Lấy thông tin order item thành công", data = result });
        }

        [HttpGet("orders/{orderId:int}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var result = await _service.GetByOrderIdAsync(orderId);
            return Ok(new { message = $"Lấy items của order #{orderId} thành công", data = result });
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateOrderItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdatePartialAsync(id, dto);
            return result == null
                ? NotFound(new { message = $"Không tìm thấy order item #{id}" })
                : Ok(new { message = "Cập nhật order item thành công", data = result });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new { message = "Đã xóa order item thành công" })
                : NotFound(new { message = $"Không tìm thấy order item #{id}" });
        }
    }
}
