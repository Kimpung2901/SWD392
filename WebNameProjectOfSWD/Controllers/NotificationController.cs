using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BLL.DTO.NotificationDto;
using BLL.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            INotificationService notificationService,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SendAsync(
            [FromBody] SendNotificationDto request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (string.IsNullOrWhiteSpace(request.DeviceToken) && string.IsNullOrWhiteSpace(request.Topic))
            {
                return BadRequest(new { error = "Device token or topic must be provided." });
            }

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            if (request.UserId.HasValue &&
                request.UserId.Value != currentUserId.Value &&
                !IsPrivilegedUser())
            {
                return Forbid();
            }

            request.UserId ??= currentUserId.Value;

            try
            {
                _logger.LogInformation("📤 Sending notification to user {UserId}", request.UserId);
                
                var result = await _notificationService.SendAsync(request, cancellationToken);

                // ✅ Kiểm tra messageId
                if (string.IsNullOrWhiteSpace(result.MessageId))
                {
                    _logger.LogWarning("⚠️ Notification saved to DB but FCM messageId is null");
                    return Ok(new
                    {
                        success = false,
                        messageId = result.MessageId,
                        notification = result.Notification,
                        warning = "Notification saved to database but push notification failed to send"
                    });
                }

                _logger.LogInformation("✅ Notification sent successfully with messageId: {MessageId}", result.MessageId);

                return Ok(new
                {
                    success = true,
                    messageId = result.MessageId,
                    notification = result.Notification
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "❌ Failed to send notification");
                return StatusCode(500, new 
                { 
                    success = false,
                    error = "Failed to send push notification",
                    details = ex.Message 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error sending notification");
                return StatusCode(500, new 
                { 
                    success = false,
                    error = "An unexpected error occurred",
                    details = ex.Message 
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications(
            [FromQuery] bool onlyUnread = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? userId = null,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var targetUserId = ResolveTargetUserId(userId, currentUserId.Value);
            if (!targetUserId.HasValue)
            {
                return Forbid();
            }

            var result = await _notificationService.GetUserNotificationsAsync(
                targetUserId.Value,
                page,
                pageSize,
                onlyUnread,
                cancellationToken);

            return Ok(new
            {
                items = result.Items,
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

        [HttpPatch("{notificationId:int}/read")]
        public async Task<IActionResult> MarkAsRead(
            int notificationId,
            [FromQuery] int? userId,
            CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var targetUserId = ResolveTargetUserId(userId, currentUserId.Value);
            if (!targetUserId.HasValue)
            {
                return Forbid();
            }

            var updated = await _notificationService.MarkAsReadAsync(
                notificationId,
                targetUserId.Value,
                cancellationToken);

            return updated ? NoContent() : NotFound();
        }

        private int? GetCurrentUserId()
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? User.FindFirstValue("UserID");

            return int.TryParse(idValue, out var id) ? id : (int?)null;
        }

        private bool IsPrivilegedUser()
        {
            return User.IsInRole("admin") || User.IsInRole("manager");
        }

        private int? ResolveTargetUserId(int? requestedUserId, int currentUserId)
        {
            if (!requestedUserId.HasValue)
            {
                return currentUserId;
            }

            if (requestedUserId.Value == currentUserId || IsPrivilegedUser())
            {
                return requestedUserId;
            }

            return null;
        }
    }
}
