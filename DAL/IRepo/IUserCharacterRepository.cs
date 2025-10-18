using DAL.Models;

namespace DAL.IRepo
{
    public interface IUserCharacterRepository
    {
        Task<List<UserCharacter>> GetAllAsync();
        Task<UserCharacter?> GetByIdAsync(int id);
        Task<List<UserCharacter>> GetByUserIdAsync(int userId);
        Task<List<UserCharacter>> GetByCharacterIdAsync(int characterId);
        Task<List<UserCharacter>> GetByPackageIdAsync(int packageId);
        Task<List<UserCharacter>> GetActiveSubscriptionsAsync(int userId);
        Task AddAsync(UserCharacter entity);
        Task UpdateAsync(UserCharacter entity);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}