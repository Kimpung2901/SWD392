using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BLL.DTO.NotificationDto;
using BLL.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
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

            var result = await _notificationService.SendAsync(request, cancellationToken);

            return Ok(new
            {
                messageId = result.MessageId,
                notification = result.Notification
            });
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
