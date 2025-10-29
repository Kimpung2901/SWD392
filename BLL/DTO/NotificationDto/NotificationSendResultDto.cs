namespace BLL.DTO.NotificationDto
{
    public class NotificationSendResultDto
    {
        public NotificationDto Notification { get; set; } = new();

        public string? MessageId { get; set; }
    }
}
