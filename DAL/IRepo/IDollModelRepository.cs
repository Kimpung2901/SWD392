using DAL.Models;

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
    }
}
