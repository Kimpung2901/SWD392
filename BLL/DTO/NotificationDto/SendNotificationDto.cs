using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.NotificationDto
{
    public class SendNotificationDto
    {
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Body is required")]
        [StringLength(500, ErrorMessage = "Body cannot exceed 500 characters")]
        public string Body { get; set; } = string.Empty;

        public Dictionary<string, string>? Data { get; set; }

        public string? Topic { get; set; }

        public string? DeviceToken { get; set; }
    }
}
