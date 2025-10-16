using BLL.DTO.DollCharacterLinkDTO;

namespace BLL.IService
{
    public interface IDollCharacterLinkService
    {
        Task<List<DollCharacterLinkDto>> GetAllAsync();
        Task<DollCharacterLinkDto?> GetByIdAsync(int id);
        Task<List<DollCharacterLinkDto>> GetByOwnedDollIdAsync(int ownedDollId);
        Task<List<DollCharacterLinkDto>> GetByUserCharacterIdAsync(int userCharacterId);
        Task<DollCharacterLinkDto?> GetActiveLinkByOwnedDollIdAsync(int ownedDollId);
        Task<DollCharacterLinkDto> CreateAsync(CreateDollCharacterLinkDto dto);
        Task<DollCharacterLinkDto?> UpdatePartialAsync(int id, UpdateDollCharacterLinkDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
    }
}