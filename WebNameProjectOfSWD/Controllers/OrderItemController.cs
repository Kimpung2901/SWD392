using BLL.DTO.OrderDTO;
using BLL.Helper;
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
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDir,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var data = await _service.GetAllAsync();
            var query = data.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLowerInvariant();
                query = query.Where(i =>
                    i.OrderItemID.ToString().Contains(term) ||
                    i.OrderID.ToString().Contains(term) ||
                    i.DollVariantID.ToString().Contains(term) ||
                    (i.DollVariantName ?? string.Empty).ToLowerInvariant().Contains(term) ||
                    i.StatusDisplay.ToLowerInvariant().Contains(term));
            }

            var total = query.Count();
            query = string.IsNullOrWhiteSpace(sortBy)
                ? query.OrderByDescending(i => i.OrderItemID)
                : query.ApplySort(sortBy, sortDir);
            query = query.ApplyPagination(page, pageSize);

            var items = query.ToList();

            return Ok(new
            {
                message = "L?y danh s�ch order items th�nh c�ng",
                items,
                pagination = BuildPagination(total, page, pageSize)
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Kh�ng t�m th?y order item #{id}" })
                : Ok(new { message = "L?y th�ng tin order item th�nh c�ng", data = result });
        }

        [HttpGet("orders/{orderId:int}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var result = await _service.GetByOrderIdAsync(orderId);
            return Ok(new { message = $"L?y items c?a order #{orderId} th�nh c�ng", data = result });
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateOrderItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdatePartialAsync(id, dto);
            return result == null
                ? NotFound(new { message = $"Kh�ng t�m th?y order item #{id}" })
                : Ok(new { message = "C?p nh?t order item th�nh c�ng", data = result });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result
                ? Ok(new { message = "D� x�a order item th�nh c�ng" })
                : NotFound(new { message = $"Kh�ng t�m th?y order item #{id}" });
        }

        private static object BuildPagination(int total, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            return new
            {
                page,
                pageSize,
                total,
                totalPages,
                hasPreviousPage = page > 1,
                hasNextPage = page < totalPages
            };
        }
    }
}
