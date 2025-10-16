using BLL.DTO.CharacterOrderDTO;

namespace BLL.IService
{
    public interface ICharacterOrderService
    {
        Task<List<CharacterOrderDto>> GetAllAsync();
        Task<CharacterOrderDto?> GetByIdAsync(int id);
        Task<List<CharacterOrderDto>> GetByCharacterIdAsync(int characterId);
        Task<List<CharacterOrderDto>> GetByPackageIdAsync(int packageId);
        Task<List<CharacterOrderDto>> GetByUserCharacterIdAsync(int userCharacterId);
        Task<CharacterOrderDto> CreateAsync(CreateCharacterOrderDto dto);
        Task<CharacterOrderDto?> UpdatePartialAsync(int id, UpdateCharacterOrderDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
    }
}