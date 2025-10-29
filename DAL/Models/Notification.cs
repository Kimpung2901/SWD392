using System;
namespace DAL.Models;

public partial class Notification
{
    public int NotificationID { get; set; }

    public int? UserID { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public string? Data { get; set; }

    public string? Topic { get; set; }

    public string? DeviceToken { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReadAt { get; set; }

    public User? User { get; set; }
}
