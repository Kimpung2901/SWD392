using System.Threading;
using System.Threading.Tasks;
using BLL.DTO.Common;
using BLL.DTO.NotificationDto;

namespace BLL.IService
{
    public interface INotificationService
    {
        Task<NotificationSendResultDto> SendAsync(
            SendNotificationDto dto,
            CancellationToken cancellationToken = default);

        Task<PagedResult<NotificationDto>> GetUserNotificationsAsync(
            int userId,
            int page,
            int pageSize,
            bool onlyUnread,
            CancellationToken cancellationToken = default);

        Task<bool> MarkAsReadAsync(
            int notificationId,
            int userId,
            CancellationToken cancellationToken = default);
    }
}
