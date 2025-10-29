using System.Threading;
using System.Threading.Tasks;
using DAL.IRepo;
using DAL.Models;

namespace DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DollDbContext _context;

        public UnitOfWork(DollDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
