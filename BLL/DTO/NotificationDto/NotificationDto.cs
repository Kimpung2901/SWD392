using System;
using System.Collections.Generic;

namespace BLL.DTO.NotificationDto
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }

        public int? UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public Dictionary<string, string> Data { get; set; } = new();

        public string? Topic { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReadAt { get; set; }
    }
}
