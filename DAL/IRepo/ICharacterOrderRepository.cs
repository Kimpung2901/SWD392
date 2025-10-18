using DAL.Models;

namespace DAL.IRepo
{
    public interface ICharacterOrderRepository
    {
        Task<List<CharacterOrder>> GetAllAsync();
        Task<CharacterOrder?> GetByIdAsync(int id);
        Task<List<CharacterOrder>> GetByUserCharacterIdAsync(int userCharacterId);
        Task<List<CharacterOrder>> GetByCharacterIdAsync(int characterId);
        Task<List<CharacterOrder>> GetByPackageIdAsync(int packageId);
        Task<List<CharacterOrder>> GetPendingOrdersAsync();
        Task AddAsync(CharacterOrder entity);
        Task UpdateAsync(CharacterOrder entity);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}