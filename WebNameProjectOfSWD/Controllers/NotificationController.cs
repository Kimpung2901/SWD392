using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // POST: /api/notification/send
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] NotificationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                return BadRequest(new { error = "Missing FCM token" });

            var messageId = await _notificationService.SendNotificationAsync(
                request.Title,
                request.Body,
                request.Token
            );

            return Ok(new { messageId });
        }
    }

    public class NotificationRequest
    {
        public string Token { get; set; } = ""; // FCM token từ Flutter
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
    }
}
