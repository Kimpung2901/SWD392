using BLL.DTO.CharacterPackageDTO;

namespace BLL.IService
{
    public interface ICharacterPackageService
    {
        Task<List<CharacterPackageDto>> GetAllAsync();
        Task<CharacterPackageDto?> GetByIdAsync(int id);
        Task<List<CharacterPackageDto>> GetByCharacterIdAsync(int characterId);
        Task<CharacterPackageDto> CreateAsync(CreateCharacterPackageDto dto);
        Task<CharacterPackageDto?> UpdatePartialAsync(int id, UpdateCharacterPackageDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
    }
}