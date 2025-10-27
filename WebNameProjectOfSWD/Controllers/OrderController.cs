using BLL.DTO.OrderDTO;
using BLL.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _service;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService service, ILogger<OrderController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Lấy tất cả orders với search/sort/pagination
    /// GET /api/orders?search=&sortBy=orderDate&sortDir=desc&page=1&pageSize=10
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAsync(search, sortBy, sortDir, page, pageSize);

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách đơn hàng thành công",
            data = result.Items,
            pagination = new
            {
                result.Page,
                result.PageSize,
                result.Total,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            }
        });
    }

    /// <summary>
    /// Lấy orders của user hiện tại với pagination
    /// GET /api/orders/my-orders?search=&sortBy=orderDate&sortDir=desc&page=1&pageSize=10
    /// </summary>
    [HttpGet("my-orders")]
    [Authorize]
    public async Task<IActionResult> GetMyOrders(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();
        var result = await _service.GetOrdersByUserIdAsync(userId, search, sortBy, sortDir, page, pageSize);

        return Ok(new
        {
            success = true,
            message = "Lấy orders của bạn thành công",
            data = result.Items,
            pagination = new
            {
                result.Page,
                result.PageSize,
                result.Total,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            }
        });
    }

    /// <summary>
    /// Lấy thông tin order theo ID
    /// GET /api/orders/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _service.GetByIdAsync(id);
        if (order == null)
            return NotFound(new { success = false, message = $"Không tìm thấy đơn hàng #{id}" });

        var userId = GetCurrentUserId();
        var isAdminOrManager = User.IsInRole("admin") || User.IsInRole("manager");

        if (!isAdminOrManager && order.UserID != userId)
            return Forbid();

        return Ok(new { success = true, data = order });
    }

    /// <summary>
    /// Lấy chi tiết order kèm items
    /// GET /api/orders/{id}/details
    /// </summary>
    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetByIdWithItems(int id)
    {
        var order = await _service.GetByIdWithItemsAsync(id);
        if (order == null)
            return NotFound(new { success = false, message = $"Không tìm thấy đơn hàng #{id}" });

        var userId = GetCurrentUserId();
        var isAdminOrManager = User.IsInRole("admin") || User.IsInRole("manager");

        if (!isAdminOrManager && order.UserID != userId)
            return Forbid();

        return Ok(new { success = true, data = order });
    }

    /// <summary>
    /// Tạo order mới
    /// POST /api/orders
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        try
        {
            var userId = GetCurrentUserId();
            if (dto.UserID != userId && !User.IsInRole("admin"))
                return Forbid();

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.OrderID },
                new { success = true, message = "Tạo đơn hàng thành công", data = created }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo đơn hàng");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật order
    /// PATCH /api/orders/{id}
    /// </summary>
    [HttpPatch("{id}")]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        try
        {
            var result = await _service.UpdatePartialAsync(id, dto);
            return result == null
                ? NotFound(new { success = false, message = $"Không tìm thấy đơn hàng #{id}" })
                : Ok(new { success = true, message = "Cập nhật đơn hàng thành công", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật đơn hàng #{Id}", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Hủy order
    /// POST /api/orders/{id}/cancel
    /// </summary>
    [HttpPost("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
                return NotFound(new { success = false, message = $"Không tìm thấy đơn hàng #{id}" });

            var userId = GetCurrentUserId();
            var isAdminOrManager = User.IsInRole("admin") || User.IsInRole("manager");

            if (!isAdminOrManager && order.UserID != userId)
                return Forbid();

            var result = await _service.CancelOrderAsync(id);
            return result
                ? Ok(new { success = true, message = "Đã hủy đơn hàng thành công" })
                : BadRequest(new { success = false, message = "Không thể hủy đơn hàng" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi hủy đơn hàng #{Id}", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa order
    /// DELETE /api/orders/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result
            ? Ok(new { success = true, message = "Đã xóa đơn hàng thành công" })
            : NotFound(new { success = false, message = $"Không tìm thấy đơn hàng #{id}" });
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst("UserID")
                    ?? User.FindFirst("sub");

        return claim != null && int.TryParse(claim.Value, out var userId) ? userId : 0;
    }
}