using BLL.DTO.OrderDTO;
using BLL.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/doll-orders")]
public class DollOrderController : ControllerBase
{
    private readonly IOrderService _service;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<DollOrderController> _logger;

    public DollOrderController(IOrderService service, IPaymentService paymentService, ILogger<DollOrderController> logger)
    {
        _service = service;
        _paymentService = paymentService;
        _logger = logger;
    }

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
            message = "Retrieved orders successfully",
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


    [HttpGet("user/{id:int}")]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> GetOrdersByUser(int id)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == 0)
            return Unauthorized();

        if (currentUserId != id)
            return Forbid();

        var result = await _service.GetOrdersByUserIdAsync(
            id,
            search: null,
            sortBy: null,
            sortDir: null,
            page: 1,
            pageSize: int.MaxValue);

        return Ok(new
        {
            success = true,
            userId = id,
            totalOrders = result.Total,
            orders = result.Items
        });
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _service.GetByIdAsync(id);
        if (order == null)
            return NotFound(new { success = false, message = $"Order #{id} not found" });

        var userId = GetCurrentUserId();
        var isAdminOrManager = User.IsInRole("admin") || User.IsInRole("manager");

        if (!isAdminOrManager && order.UserID != userId)
            return Forbid();

        return Ok(new { success = true, data = order });
    }


    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        try
        {
           
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định user từ token" });
            }

        
            var created = await _service.CreateAsync(dto, userId);

         
            var paymentResult = await _paymentService.CreateMoMoPaymentAsync(
                created.TotalAmount,
                created.OrderID,
                null,
                ct);

            if (!paymentResult.Success)
            {
                _logger.LogError("Failed to create payment for order {OrderId}: {Message}", 
                    created.OrderID, paymentResult.Message);
                
                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    new
                    {
                        success = false,
                        message = "Không thể tạo thanh toán MoMo",
                        order = created,
                        paymentError = paymentResult.Message
                    });
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.OrderID },
                new
                {
                    success = true,
                    message = "Order created successfully",
                    data = created,
                    payment = new
                    {
                        paymentId = paymentResult.PaymentId,
                        payUrl = paymentResult.PayUrl
                    }
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "AllowEdit")] // ✅ Đúng
    public async Task<IActionResult> UpdatePartial(int id, [FromBody] UpdateOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        try
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
                return NotFound(new { success = false, message = $"Order #{id} not found" });

            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("admin");

            if (!isAdmin && order.UserID != userId)
                return Forbid();

            var result = await _service.UpdatePartialAsync(id, dto);
            return Ok(new { success = true, message = "Order updated successfully", data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order #{Id}", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }


    [HttpPost("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
                return NotFound(new { success = false, message = $"Order #{id} not found" });

            var userId = GetCurrentUserId();
            var isAdminOrManager = User.IsInRole("admin") || User.IsInRole("manager");

            if (!isAdminOrManager && order.UserID != userId)
                return Forbid();

            var result = await _service.CancelOrderAsync(id);
            return result
                ? Ok(new { success = true, message = "Order cancelled successfully" })
                : BadRequest(new { success = false, message = "Unable to cancel order" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order #{Id}", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }


    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result
            ? Ok(new { success = true, message = "Order deleted successfully" })
            : NotFound(new { success = false, message = $"Order #{id} not found" });
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst("UserID")
                    ?? User.FindFirst("sub");

        return claim != null && int.TryParse(claim.Value, out var userId) ? userId : 0;
    }
}


