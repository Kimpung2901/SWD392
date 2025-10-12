using DAL.Models;
using BLL.DTO.CharacterPackageDTO;

namespace BLL.IService
{
    public interface ICharacterPackageService
    {
        Task<List<CharacterPackage>> GetAllAsync();
        Task<CharacterPackage?> GetByIdAsync(int id);
        Task<List<CharacterPackage>> GetByCharacterIdAsync(int characterId);
        Task<CharacterPackage> CreateAsync(CreateCharacterPackageDto dto);
        Task<CharacterPackage?> UpdatePartialAsync(int id, UpdateCharacterPackageDto dto);
        Task<bool> DeleteAsync(int id);
    }
}