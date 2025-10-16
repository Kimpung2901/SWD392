using DAL.Models;

namespace DAL.IRepo
{
    public interface ICharacterOrderRepository
    {
        Task<List<CharacterOrder>> GetAllAsync();
        Task<CharacterOrder?> GetByIdAsync(int id);
        Task<List<CharacterOrder>> GetByCharacterIdAsync(int characterId);
        Task<List<CharacterOrder>> GetByPackageIdAsync(int packageId);
        Task<List<CharacterOrder>> GetByUserCharacterIdAsync(int userCharacterId);
        Task AddAsync(CharacterOrder entity);
        Task UpdateAsync(CharacterOrder entity);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}