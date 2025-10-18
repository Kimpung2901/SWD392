using BLL.DTO.UserCharacterDTO;

namespace BLL.IService
{
    public interface IUserCharacterService
    {
        Task<List<UserCharacterDto>> GetAllAsync();
        Task<UserCharacterDto?> GetByIdAsync(int id);
        Task<List<UserCharacterDto>> GetByUserIdAsync(int userId);
        Task<List<UserCharacterDto>> GetByCharacterIdAsync(int characterId);
        Task<List<UserCharacterDto>> GetByPackageIdAsync(int packageId);
        Task<List<UserCharacterDto>> GetActiveSubscriptionsAsync(int userId);
        Task<UserCharacterDto> CreateAsync(CreateUserCharacterDto dto);
        Task<UserCharacterDto?> UpdatePartialAsync(int id, UpdateUserCharacterDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> RenewSubscriptionAsync(int userCharacterId);
    }
}