using DAL.Models;

namespace DAL.IRepo
{
    public interface IOwnedDollRepository
    {
        Task<List<OwnedDoll>> GetAllAsync();
        Task<OwnedDoll?> GetByIdAsync(int id);
        Task<List<OwnedDoll>> GetByUserIdAsync(int userId);
        Task<List<OwnedDoll>> GetByDollVariantIdAsync(int dollVariantId);
        Task AddAsync(OwnedDoll entity);
        Task UpdateAsync(OwnedDoll entity);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}