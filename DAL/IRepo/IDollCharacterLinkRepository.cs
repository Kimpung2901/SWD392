using DAL.Models;

namespace DAL.IRepo
{
    public interface IDollCharacterLinkRepository
    {
        Task<List<DollCharacterLink>> GetAllAsync();
        Task<DollCharacterLink?> GetByIdAsync(int id);
        Task<List<DollCharacterLink>> GetByOwnedDollIdAsync(int ownedDollId);
        Task<List<DollCharacterLink>> GetByUserCharacterIdAsync(int userCharacterId);
        Task<DollCharacterLink?> GetActiveLinkByOwnedDollIdAsync(int ownedDollId);
        Task AddAsync(DollCharacterLink entity);
        Task UpdateAsync(DollCharacterLink entity);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}