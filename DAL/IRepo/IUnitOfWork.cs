using System.Threading;
using System.Threading.Tasks;

namespace DAL.IRepo
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
