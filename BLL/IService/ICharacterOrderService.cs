using BLL.DTO.CharacterOrderDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.IService
{
    public interface ICharacterOrderService
    {
        Task<List<CharacterOrderDto>> GetAllAsync();
        Task<CharacterOrderDto?> GetByIdAsync(int id);
        Task<List<CharacterOrderDto>> GetByUserIdAsync(int userId);
        Task<List<CharacterOrderDto>> GetByUserCharacterIdAsync(int userCharacterId);
        Task<List<CharacterOrderDto>> GetByCharacterIdAsync(int characterId);
        Task<List<CharacterOrderDto>> GetByPackageIdAsync(int packageId);
        Task<List<CharacterOrderDto>> GetPendingOrdersAsync();
        Task<CharacterOrderDto> CreateAsync(CreateCharacterOrderDto dto, int userId); // ✅ Thêm userId parameter
        Task<CharacterOrderDto?> UpdatePartialAsync(int id, UpdateCharacterOrderDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> CompleteOrderAsync(int id);
        Task<bool> CancelOrderAsync(int id);
    }
}