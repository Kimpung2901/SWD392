using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.IRepo
{
    public interface INotificationRepository
    {
        IQueryable<Notification> Query();

        Task<Notification?> GetByIdAsync(int notificationId, CancellationToken cancellationToken = default);

        Task<Notification> AddAsync(Notification notification, CancellationToken cancellationToken = default);

        Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
    }
}
