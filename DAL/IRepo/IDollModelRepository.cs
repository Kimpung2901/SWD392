using DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.IRepo
{
    public interface IDollModelRepository
    {
        Task<List<DollModel>> GetAllAsync();
        Task<DollModel?> GetByIdAsync(int id);
        Task<List<DollModel>> GetByTypeIdAsync(int dollTypeId);
        Task AddAsync(DollModel entity);
        Task UpdateAsync(DollModel entity);
        Task SoftDeleteAsync(int id);
        Task HardDeleteAsync(int id);
        IQueryable<DollModel> Query();
    }
}
