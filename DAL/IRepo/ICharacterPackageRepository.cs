using DAL.Models;

namespace DAL.IRepo
{
    public interface ICharacterPackageRepository
    {
        Task<List<CharacterPackage>> GetAllAsync();
        Task<CharacterPackage?> GetByIdAsync(int id);
        Task<List<CharacterPackage>> GetByCharacterIdAsync(int characterId);
        Task AddAsync(CharacterPackage entity);
        Task UpdateAsync(CharacterPackage entity);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}