using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.IRepo
{
    public interface IDollTypeRepository
    {
        Task<List<DollType>> GetAllAsync();
        Task<DollType?> GetByIdAsync(int id);
        Task AddAsync(DollType entity);
        Task<DollType?> UpdateAsync(DollType entity);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
    }
}
