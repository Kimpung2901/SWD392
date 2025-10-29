using System.Threading;
using System.Threading.Tasks;
using BLL.DTO.NotificationDto;
using BLL.IService;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("send")]
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

            var result = await _notificationService.SendAsync(request, cancellationToken);

            return Ok(new
            {
                messageId = result.MessageId,
                notification = result.Notification
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications(
            [FromQuery] int userId,
            [FromQuery] bool onlyUnread = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
            {
                return BadRequest(new { error = "userId must be greater than zero." });
            }

            var result = await _notificationService.GetUserNotificationsAsync(
                userId,
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
            [FromQuery] int userId,
            CancellationToken cancellationToken)
        {
            if (userId <= 0)
            {
                return BadRequest(new { error = "userId must be greater than zero." });
            }

            var updated = await _notificationService.MarkAsReadAsync(notificationId, userId, cancellationToken);

            return updated ? NoContent() : NotFound();
        }
    }
}
