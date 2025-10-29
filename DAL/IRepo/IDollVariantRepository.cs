using DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.IRepo
{
    public interface IDollVariantRepository
    {
        Task<List<DollVariant>> GetAllAsync();
        Task<DollVariant?> GetByIdAsync(int id);
        Task<List<DollVariant>> GetByDollModelIdAsync(int dollModelId);
        Task AddAsync(DollVariant entity);
        Task UpdateAsync(DollVariant entity);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
        IQueryable<DollVariant> Query();
    }
}
