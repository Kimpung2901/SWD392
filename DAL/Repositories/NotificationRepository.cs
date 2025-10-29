using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DollDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationRepository(DollDbContext db, IUnitOfWork unitOfWork)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Notification> Query()
        {
            return _db.Notifications.AsNoTracking();
        }

        public async Task<Notification?> GetByIdAsync(int notificationId, CancellationToken cancellationToken = default)
        {
            return await _db.Notifications
                .FirstOrDefaultAsync(n => n.NotificationID == notificationId, cancellationToken);
        }

        public async Task<Notification> AddAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _db.Notifications.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return notification;
        }

        public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            _db.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
