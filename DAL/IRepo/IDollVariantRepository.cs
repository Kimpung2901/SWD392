using DAL.Models;

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
    }
}