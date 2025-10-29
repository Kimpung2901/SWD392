using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BLL.DTO.Common;
using BLL.DTO.NotificationDto;
using BLL.IService;
using DAL.IRepo;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotificationEntity = DAL.Models.Notification;

namespace BLL.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository repository,
            ILogger<NotificationService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<NotificationSendResultDto> SendAsync(
            SendNotificationDto dto,
            CancellationToken cancellationToken = default)
        {
            var notification = new NotificationEntity
            {
                UserID = dto.UserId,
                Title = dto.Title,
                Body = dto.Body,
                Topic = dto.Topic,
                DeviceToken = dto.DeviceToken,
                Data = dto.Data != null && dto.Data.Count > 0
                    ? JsonSerializer.Serialize(dto.Data)
                    : null,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            notification = await _repository.AddAsync(notification, cancellationToken);

            string? messageId = null;

            if (!string.IsNullOrWhiteSpace(dto.DeviceToken) || !string.IsNullOrWhiteSpace(dto.Topic))
            {
                var message = new Message
                {
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = dto.Title,
                        Body = dto.Body
                    },
                    Data = dto.Data ?? new Dictionary<string, string>()
                };

                if (!string.IsNullOrWhiteSpace(dto.DeviceToken))
                {
                    message.Token = dto.DeviceToken;
                }
                else if (!string.IsNullOrWhiteSpace(dto.Topic))
                {
                    message.Topic = dto.Topic;
                }

                try
                {
                    var messaging = FirebaseMessaging.DefaultInstance;
                    messageId = await messaging.SendAsync(message, cancellationToken);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex,
                        "Firebase messaging is not available. Notification {NotificationId} stored without push delivery.",
                        notification.NotificationID);
                }
                catch (FirebaseMessagingException ex)
                {
                    _logger.LogError(ex,
                        "Firebase messaging failed for notification {NotificationId}.",
                        notification.NotificationID);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Unexpected error when sending notification {NotificationId}.",
                        notification.NotificationID);
                }
            }

            return new NotificationSendResultDto
            {
                Notification = Map(notification),
                MessageId = messageId
            };
        }

        public async Task<PagedResult<NotificationDto>> GetUserNotificationsAsync(
            int userId,
            int page,
            int pageSize,
            bool onlyUnread,
            CancellationToken cancellationToken = default)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var query = _repository
                .Query()
                .Where(n => !n.UserID.HasValue || n.UserID == userId);

            if (onlyUnread)
            {
                query = query.Where(n => !n.IsRead);
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<NotificationDto>
            {
                Items = items.Select(Map).ToList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> MarkAsReadAsync(
            int notificationId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            var notification = await _repository.GetByIdAsync(notificationId, cancellationToken);

            if (notification == null)
            {
                return false;
            }

            if (notification.UserID.HasValue && notification.UserID != userId)
            {
                return false;
            }

            if (notification.IsRead)
            {
                return true;
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _repository.UpdateAsync(notification, cancellationToken);
            return true;
        }

        private NotificationDto Map(NotificationEntity notification)
        {
            var data = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(notification.Data))
            {
                try
                {
                    data = JsonSerializer.Deserialize<Dictionary<string, string>>(notification.Data)
                           ?? new Dictionary<string, string>();
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex,
                        "Unable to deserialize notification data for NotificationID {NotificationId}",
                        notification.NotificationID);
                }
            }

            return new NotificationDto
            {
                NotificationId = notification.NotificationID,
                UserId = notification.UserID,
                Title = notification.Title,
                Body = notification.Body,
                Data = data,
                Topic = notification.Topic,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt
            };
        }
    }
}
