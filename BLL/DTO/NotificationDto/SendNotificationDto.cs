using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.NotificationDto
{
    public class SendNotificationDto
    {
        public int? UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        public string? DeviceToken { get; set; }

        public string? Topic { get; set; }

        public Dictionary<string, string>? Data { get; set; }
    }
}
